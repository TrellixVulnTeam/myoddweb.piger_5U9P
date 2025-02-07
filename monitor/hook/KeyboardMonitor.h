#pragma once
#include <map>

class KeyboardMonitor
{
public:
  KeyboardMonitor( HANDLE hModule );
  ~KeyboardMonitor(void);

protected:
  //  the function called for all the message
  static LRESULT CALLBACK CallLowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);

public:
  bool AddHwnd( HWND hWnd, WPARAM vKey);
  bool RemoveHwnd( HWND hWnd );
  bool RejectKeyboadInputs(bool bReject  );
  bool RejectKeyboadInputs( )const;

protected:
  //  key structure so we can prevent similar keys from repeating too often
  //  there is nothing stopping an app from requesting the same key over and over
  //  that does not mean that the key was pressed more than once.
  struct LAST_KEY
  {
    UINT  m_msg;
    DWORD m_dwKey;
    DWORD m_tick;

    // ---------------------------------
    DWORD tick() const{ return m_tick;}

    // constructor
    LAST_KEY() : m_msg(0), m_dwKey(0), m_tick( 0 ){ }
    //  copy contructor
    LAST_KEY(const LAST_KEY& lk ){ *this = lk;}
    LAST_KEY(UINT m, DWORD k, DWORD t =0 ) : m_msg(m), m_dwKey(k), m_tick( t ){}

    //  we don't compare the time!!
    bool Similar( const DWORD msg, const DWORD dwKey ) const
    { 
      return (m_msg == msg && m_dwKey == dwKey);
    }

    //  operator equal.
    const LAST_KEY& operator =(const LAST_KEY& lk )
    { 
      if( this != &lk)
      { 
        m_msg   = lk.m_msg; 
        m_dwKey = lk.m_dwKey; 
        m_tick  = lk.m_tick;
      } 
      return *this;
    }
  };

protected:
  UINT UWM_KEYBOARD_CHAR;
  UINT UWM_KEYBOARD_UP;
  UINT UWM_KEYBOARD_DOWN;

protected:
  HANDLE m_hModule;
  /**
   * The last key that was set.
   */
  LAST_KEY	m_lk;

  HHOOK GetHhook() const{ return m_hhook;} 
  void SetHhook(const HHOOK hh ){ m_hhook = hh;} 
  bool CreateHooks();
  bool ClearHooks();

  bool IsSpecialKey( WPARAM wParam ) const;
  LRESULT HandleMessage( const UINT msg, const WPARAM wParam, const LPARAM lParam );
  LAST_KEY GetLastKey( DWORD dwCurrentMilli ) const;
  void SetLastKey( const LAST_KEY& lk );

  bool m_bRejectKeyBoardInputs;
  HHOOK m_hhook;

  // the keys and windows.
  std::map< HWND, WPARAM > m_mapWindows;

  /**
   * return if we are handling that code or not.
   */
  static bool HandleHook (int nCode );
};

extern KeyboardMonitor* keyboardMonitor;
