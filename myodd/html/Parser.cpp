#include "StdAfx.h"
#include "Parser.h"

static Tokens m_tokens;

Parser::Parser( const HDC hdc ) : 
  mSaveDC( -1 ),
  mFont( nullptr )
{
  Init( hdc );
}

Parser::~Parser()
{
  assert( mFont == nullptr ); //  forgot to call deinit?
  Clear();
}

/**
 * \brief Delete all the tags/text that we have/
 */
void Parser::Clear()
{
  for (auto it = m_data.begin(); it != m_data.end(); ++it)
  {
    delete *it;
  }
  m_data.clear();
}

/**
 * \brief Parse a string and build an array of HTML tags/text and so on.
 * \param lpString the string we are parsing.
 * \param nCount the number of characters we want to parse.
 * \return const Parser::HTML_CONTAINER& a container with all html the tags and text.
 */
const Parser::HTML_CONTAINER& Parser::Parse(const MYODD_CHAR* lpString, int nCount )
{
  Clear();
  if( !lpString )
  {
    return Tree();
  }

  // use the len of the string
  nCount = nCount < 0 ? _tcslen( lpString ) : nCount;

  auto limit = lpString+nCount;
  auto ptr = lpString;
  auto begin  = ptr;
  auto end    = ptr;
  while (*ptr && end != limit ) 
  {
    // look for the opening tag '<'
    if ((end = _tcschr (begin, '<')) != nullptr)
    {
      // assign the value from wherever we come from to here.
      // if the tag is the very first one, then there is no need to assign that value.
      end = end > limit ? limit : end;
      Add( begin, end, false );
      begin = end;

      // look for the closing tag.
      // if we have reached our limit no need to look further.
      if ( begin != limit )
      {
        if ((end = _tcschr (begin, '>')) != nullptr)
        {
          ++end;  //  include the closing tag.
          end = end > limit ? limit : end;
          Add( begin, end, true );
          ptr = begin = end;
        }
        else
        {
          // this is not very good, we have no closing '>'
          // this will not render properly.
          Add( begin, limit, false );
          break;
        }
      }
    }
    // no more attribute, just go to the end,
    else
    {
      Add( begin, limit, false );
      break;
    }
  }
  return Tree();
}

/**
 * \brief Add a string to the array of HTMLDATA 
 * \param begin the start of the string
 * \param end the end of the string 
 * \param isHtmlTag if this is a tag or not.
 */
void Parser::Add(const MYODD_CHAR* begin, const MYODD_CHAR* end, const bool isHtmlTag )
{
  // an empty string, no need to do it.
  if( begin == end )
    return;

  const auto hd = new HTMLDATA;
  hd->mIsHtmlTag = isHtmlTag;

  if( isHtmlTag )
  {
    assert( *begin == '<' );
    begin++;    //  '<'

    assert( *(end-1) == '>' );
    end--;      //  '>'
    hd->mIsEnd = *begin == _T('/') ? true : false;
    if( hd->mIsEnd )
    {
      begin++;  //  '/'
    }

    // just get the name skip the attributes.
    const MYODD_CHAR* space = _tcschr (begin, ' ');
    if( space && space < end )
    {
      hd->attributes.assign( space+1, end );
      end = space;
    }

    //hd->text.assign( begin, end );
    assert( end - begin > 0 );  // how can the tag be empty??
    hd->text.assign(end - begin, '\0');
    transform(begin, end, hd->text.begin(), ::tolower);
    
    // look for the matching token for that tag
    // the token is what does the basic string transformation.
    for( auto it = m_tokens.begin();
      it != m_tokens.end();
      ++it )
    {
      if( (*it)->IsToken( hd->text.c_str(), (end-begin) ))
      {
        hd->mToken = *it;
        break;
      }
    }

    // if we do not have a token it means that we are not going to parse it properly.
    // the text in the tag will not be displayed.
    // did you mean to use &gt; (>) and &lt; (<)?
    assert( hd->mToken );
  }
  else
  {
    // this is not a tag so we just assign the text.
    // we might want to do more formating here.
    hd->text.assign( begin, end );

    // special chars are done straight away
    myodd::strings::Replace( hd->text, _T("&nbsp;"), _T(" "), false );
    myodd::strings::Replace( hd->text, _T("&lt;"), _T("<"), false);
    myodd::strings::Replace( hd->text, _T("&gt;"), _T(">"), false);
    myodd::strings::Replace( hd->text, _T("&amp;"), _T("&"), false);
    myodd::strings::Replace( hd->text, _T("&deg;"),  MYODD_STRING(1, MYODD_CHAR(176)), false);    //  degree
    myodd::strings::Replace( hd->text, _T("&plusmn;"),  MYODD_STRING(1, MYODD_CHAR(177)), false); //  Plus/minus symbol
  }

  // add this to the list.
  m_data.push_back( hd );
}

void CalculateSmartDimensions( SIZE& size, HDC hDCScreen, const MYODD_CHAR* szText, int nLen )
{
  if( nLen == -1 )
  {
    nLen = szText ? _tcslen(szText) : 0;
  }
  auto& lHeight = size.cy;
  auto& lWidth = size.cx;

  const auto hFont = static_cast<HFONT>(::GetCurrentObject(hDCScreen, OBJ_FONT));
  const auto hDCMem=CreateCompatibleDC(hDCScreen);

  const auto hFontOld=static_cast<HFONT>(SelectObject(hDCMem, hFont));

  //calculate the width of the string using the classic way
  SIZE sizeText;
  GetTextExtentPoint32(hDCMem, szText, nLen, &sizeText);

  //Or calculate like this. But it's no better than using GetTextExtentPoint32
  //RECT rectResult={0,0,0,0};
  //DrawText(hDCMem,szText,-1,&rectResult,DT_CALCRECT|DT_LEFT|DT_TOP|DT_SINGLELINE|DT_NOPREFIX);
  //sizeText.cx=rectResult.right-rectResult.left;


  lHeight=sizeText.cy;//lHeight==0 if the text is empty, so try GetTextMetrics below!
  lWidth=sizeText.cx;

  if(nullptr==szText || _T('\0')==szText[0])
  {
    TEXTMETRIC tm;
    GetTextMetrics(hDCMem,&tm);//this helps if the text is empty!
    lHeight=tm.tmHeight;//no space between the lines, doesn't need +tm.tmExternalLeading;

    SelectObject(hDCMem,hFontOld);
    DeleteDC(hDCMem);
    return;
  }

  // if the last item is a space then we cannot calculate its width
  if( szText[-1+nLen] == _T(' '))
  {
    SelectObject(hDCMem,hFontOld);
    DeleteDC(hDCMem);
    return;
  }

  //RECT rect={0,0,lWidth+2*size.cx,lHeight};
  // August 17, 2006
  // Set the right-limit of the bounding rectangle to be scanned to just 
  // 'sizeText.cx+widthOfTheLastCharacter', as suggested by 'oupoi'. 

  SIZE sizeLastCharacter;
  GetTextExtentPoint32(hDCMem, &szText[-1+nLen], 1, &sizeLastCharacter);

  //enough to fit the text
  RECT rect={0,0,lWidth+sizeLastCharacter.cx,lHeight};

  const auto hBitmap= CreateCompatibleBitmap ( hDCMem,
    rect.right-rect.left,
    rect.bottom-rect.top);

  const auto hOldBitmap=static_cast<HBITMAP>(SelectObject(hDCMem, hBitmap));

  //fill with white
  FillRect(hDCMem,&rect,static_cast<HBRUSH>(GetStockObject(WHITE_BRUSH)));
  DrawText(hDCMem,szText,nLen,&rect,DT_LEFT|DT_TOP|DT_SINGLELINE|DT_NOPREFIX);

  auto iXmax=0;
  auto bFound=false;
  for(int x=rect.right-1;x>=0 && !bFound ;x--)	
  {
    for (auto y=0;y<=rect.bottom-1 && !bFound;y++)	
    {

      const auto rgbColor=GetPixel(hDCMem,x,y);

      if(rgbColor!=RGB(255,255,255))
      {
        //found a non-white pixel, save the horizontal position
        //and exit the loop. Job finished.
        iXmax=x;
        bFound=true;
      }
    }//endfor
  }//endfor	

  //here we have the width of the painted text
  lWidth=iXmax+1;//+1 because we use 0-based indexes

  SelectObject(hDCMem,hFontOld);
  SelectObject(hDCMem,hOldBitmap);
  DeleteObject(hBitmap);
  DeleteDC(hDCMem);
}

void Parser::Init( const HDC hdc )
{
  // save the dc so we can restore it when we are done.
  mSaveDC = SaveDC( hdc );

  // get the current font
  const auto hFont = static_cast<HFONT>(::GetCurrentObject(hdc, OBJ_FONT));
  
  // and the current log font.
  ::GetObject(hFont, sizeof(LOGFONT), &mLogFont );
}

void Parser::DeInit( const HDC hdc )
{
  if( -1 != mSaveDC )
  {
    RestoreDC( hdc, mSaveDC );
    mSaveDC = -1;
  }

  // delete the old font.
  if( mFont )
  {
    DeleteObject( mFont );
    mFont = nullptr;
  }
}

SIZE Parser::Apply( const HDC hdc, 
                    const Parser::HTMLDATA* hd, 
                    RECT& rect,
                    const RECT& givenRect,
                    const UINT uFormat 
                  )
{
  const auto lpString = hd->text.c_str();
  const auto lpStringLen  = hd->text.length();

  SIZE size = {0};

  // are we a tag?
  if( hd->mToken )
  {
    // this function does single lines only
    // but when asked to parse from beginning to end we are 
    // given the token that caused us to go to the next line
    auto lf = GetCurrentLogFont();
    if( hd->mIsEnd )
    {
      hd->mToken->pop( lf );
      PopFont( hdc, lf );
    }
    else
    {
      hd->mToken->push( lf );
      PushFont( hdc, lf );
    }
  }

  // we only output if we are not a string 
  if( lpStringLen > 0 && !hd->mIsHtmlTag )
  {
    // in case the user has ellipses we need to use the rectangle 
    // as it was given to us.
    // RECT rc = {0};
    // memcpy( &rc, &rect, sizeof(RECT) );
    // DrawText( hdc, lpString, lpStringLen, &rc, uFormat | DT_CALCRECT );

    if( (uFormat & DT_CALCRECT) == 0 )
    {
      DrawText( hdc, lpString, lpStringLen, &rect, uFormat );
    }

    SIZE calcSize = {0};
    CalculateSmartDimensions( calcSize, hdc, lpString, lpStringLen );

    // given the size of the rectangle, update the various sizes.
    // they will be used later to make sure that we have the right sizes.
    int nWidth = calcSize.cx;
    if(  rect .left+nWidth > rect .right )
    {
      if( givenRect.right - givenRect.left > 0 )
      {
        nWidth =  rect .right - rect .left;
      }
    }
    rect .left += nWidth;
    rect .left = rect .left > rect .right ? rect .right : rect .left;
    size.cx+= nWidth;

    // update the return values.
    int nHeight = calcSize.cy;
    if(  rect .top+nHeight > rect .bottom )
    {
      if( givenRect.bottom - givenRect.top > 0 )
      {
        nHeight =  rect .bottom - rect .top;
      }
    }
    size.cy = size.cy< nHeight? nHeight : size.cy;          
  }

  // return the size of what we just did.
  return size;
}

void Parser::PushFont( HDC hdc, const LOGFONT& lf )
{
  ApplyFont( hdc, lf );
}

void Parser::PopFont( HDC hdc, const LOGFONT& lf )
{
  ApplyFont( hdc, lf );
}

void Parser::ApplyFont( HDC hdc, const LOGFONT& lf )
{
  // if the current style is not the same as the last one
  // then we don't need to actually change anything.
  if( 0 == memcmp( &lf, &GetCurrentLogFont(), sizeof(LOGFONT)))
  {
    return;
  }

  // do we already have a font?
  const auto hFont = ::CreateFontIndirect( &lf );

  // select that font
  SelectObject(hdc, hFont );

  if( mFont )
  {
    DeleteObject( mFont );
  }

  // this is the new font.
  mFont = hFont;

  // this is the new log font
  mLogFont = lf;
}