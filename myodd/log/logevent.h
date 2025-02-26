#pragma once

#include "../common/includes.h"
#include "../notif/notif.h"

#include "logfile.h"
#include "logtype.h"
#include <vector>

namespace myodd {
namespace log {
  // the actual log class
  class LogEvent : public myodd::notif::Notifications<LogEventCallback>
  {
  public:
    virtual ~LogEvent(void);

    const MYODD_STRING& GetCurrentLogFile() const {
      return m_logFile.GetCurrentLogFile();
    }

    bool Initialise(const std::wstring& wPath, const std::wstring& wPrefix, const std::wstring& wExtention, size_t maxFileSize);
    bool Initialised() const;

  protected:

    LogEvent(void);
    const LogEvent& operator=(const LogEvent&);      // Prevent assignment

    bool _initialised;

    //  all the messages.
    class _LogMessage
    {
    public:
      _LogMessage(LogType lt = LogType::None, const MYODD_CHAR* lpMessage = NULL) :
        _uiType(lt),
        _szLogLine(lpMessage)
      {
      }
      _LogMessage(const _LogMessage& lm) { Copy(lm); }
      const _LogMessage& operator=(const _LogMessage& lm) {
        Copy(lm);
        return lm;
      }

      const MYODD_CHAR* GetMessage() const { return _szLogLine.c_str(); }
      LogType GetType() const { return _uiType; }
    protected:
      void Copy(const _LogMessage& lm)
      {
        if (this == &lm)
          return;

        _szLogLine = lm._szLogLine;
        _uiType = lm._uiType;
      }
      MYODD_STRING _szLogLine;
      LogType _uiType;
    };

    // the last few messages.
    std::vector<_LogMessage> m_logMessages;

  protected:
    myodd::log::LogFile m_logFile;
    bool LogToFile(LogType uiType, const MYODD_CHAR* pszLine);
    void LogInLockedThread(LogType uiType, const MYODD_CHAR* pszLine);

  public:
    // notification messages
    bool AddNotif(const LogEventCallback& fnNotif, MYODD_LPARAM lParam, size_t iSendLast);
    bool RemoveNotif(const LogEventCallback& fnNotif, MYODD_LPARAM lParam);

    // the actual log messages.
    void Log(LogType uFlags, const MYODD_CHAR* pszFmt, va_list argp);
    void Log(LogType uFlags, const MYODD_CHAR* pszFmt );
    static LogEvent& Instance();
  };

} //  logevent
} //  myodd
