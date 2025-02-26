/* src/config.h.  Generated from config.h.in by configure.  */
/* src/config.h.in.  Generated from configure.ac by autoheader.  */

/* PCRE2 is written in Standard C, but there are a few non-standard things it
can cope with, allowing it to run on SunOS4 and other "close to standard"
systems.

In environments that support the GNU autotools, config.h.in is converted into
config.h by the "configure" script. In environments that use CMake,
config-cmake.in is converted into config.h. If you are going to build PCRE2 "by
hand" without using "configure" or CMake, you should copy the distributed
config.h.generic to config.h, and edit the macro definitions to be the way you
need them. You must then add -DHAVE_CONFIG_H to all of your compile commands,
so that config.h is included at the start of every source.

Alternatively, you can avoid editing by using -D on the compiler command line
to set the macro values. In this case, you do not have to set -DHAVE_CONFIG_H,
but if you do, default values will be taken from config.h for non-boolean
macros that are not defined on the command line.

Boolean macros such as HAVE_STDLIB_H and SUPPORT_PCRE2_8 should either be defined
(conventionally to 1) for TRUE, and not defined at all for FALSE. All such
macros are listed as a commented #undef in config.h.generic. Macros such as
MATCH_LIMIT, whose actual value is relevant, have defaults defined, but are
surrounded by #ifndef/#endif lines so that the value can be overridden by -D.

PCRE2 uses memmove() if HAVE_MEMMOVE is defined; otherwise it uses bcopy() if
HAVE_BCOPY is defined. If your system has neither bcopy() nor memmove(), make
sure both macros are undefined; an emulation function will then be used. */

/* By default, the \R escape sequence matches any Unicode line ending
   character or sequence of characters. If BSR_ANYCRLF is defined (to any
   value), this is changed so that backslash-R matches only CR, LF, or CRLF.
   The build-time default can be overridden by the user of PCRE2 at runtime.
   */
/* #undef BSR_ANYCRLF */

/* If you are compiling for a system that uses EBCDIC instead of ASCII
   character codes, define this macro to any value. When EBCDIC is set, PCRE2
   assumes that all input strings are in EBCDIC. If you do not define this
   macro, PCRE2 will assume input strings are ASCII or UTF-8/16/32 Unicode. It
   is not possible to build a version of PCRE2 that supports both EBCDIC and
   UTF-8/16/32. */
/* #undef EBCDIC */

/* In an EBCDIC environment, define this macro to any value to arrange for the
   NL character to be 0x25 instead of the default 0x15. NL plays the role that
   LF does in an ASCII/Unicode environment. */
/* #undef EBCDIC_NL25 */

/* Define to 1 if you have the `bcopy' function. */
/* #undef HAVE_BCOPY */

/* Define to 1 if you have the <bzlib.h> header file. */
/* #undef HAVE_BZLIB_H */

/* Define to 1 if you have the <dirent.h> header file. */
/* #undef HAVE_DIRENT_H */

/* Define to 1 if you have the <dlfcn.h> header file. */
/* #undef HAVE_DLFCN_H */

/* Define to 1 if you have the <editline/readline.h> header file. */
/* #undef HAVE_EDITLINE_READLINE_H */

/* Define to 1 if you have the <edit/readline/readline.h> header file. */
/* #undef HAVE_EDIT_READLINE_READLINE_H */

/* Define to 1 if you have the <inttypes.h> header file. */
/* #undef HAVE_INTTYPES_H */

/* Define to 1 if you have the <limits.h> header file. */
/* #undef HAVE_LIMITS_H */

/* Define to 1 if you have the `memmove' function. */
/* #undef HAVE_MEMMOVE */

/* Define to 1 if you have the <memory.h> header file. */
/* #undef HAVE_MEMORY_H */

/* Define if you have POSIX threads libraries and header files. */
/* #undef HAVE_PTHREAD */

/* Have PTHREAD_PRIO_INHERIT. */
/* #undef HAVE_PTHREAD_PRIO_INHERIT */

/* Define to 1 if you have the <readline/history.h> header file. */
/* #undef HAVE_READLINE_HISTORY_H */

/* Define to 1 if you have the <readline/readline.h> header file. */
/* #undef HAVE_READLINE_READLINE_H */

/* Define to 1 if you have the <stdint.h> header file. */
/* #undef HAVE_STDINT_H */

/* Define to 1 if you have the <stdlib.h> header file. */
/* #undef HAVE_STDLIB_H */

/* Define to 1 if you have the `strerror' function. */
/* #undef HAVE_STRERROR */

/* Define to 1 if you have the <strings.h> header file. */
/* #undef HAVE_STRINGS_H */

/* Define to 1 if you have the <string.h> header file. */
/* #undef HAVE_STRING_H */

/* Define to 1 if you have the <sys/stat.h> header file. */
/* #undef HAVE_SYS_STAT_H */

/* Define to 1 if you have the <sys/types.h> header file. */
/* #undef HAVE_SYS_TYPES_H */

/* Define to 1 if you have the <sys/wait.h> header file. */
/* #undef HAVE_SYS_WAIT_H */

/* Define to 1 if you have the <unistd.h> header file. */
/* #undef HAVE_UNISTD_H */

/* Define to 1 if the compiler supports simple visibility declarations. */
/* #undef HAVE_VISIBILITY */

/* Define to 1 if you have the <windows.h> header file. */
/* #undef HAVE_WINDOWS_H */

/* Define to 1 if you have the <zlib.h> header file. */
/* #undef HAVE_ZLIB_H */

/* PCRE2 uses recursive function calls to handle backtracking while matching.
   This can sometimes be a problem on systems that have stacks of limited
   size. Define HEAP_MATCH_RECURSE to any value to get a version that doesn't
   use recursion in the match() function; instead it creates its own stack by
   steam using memory from the heap. For more detail, see the comments and
   other stuff just above the match() function. */
/* #undef HEAP_MATCH_RECURSE */

/* The value of LINK_SIZE determines the number of bytes used to store links
   as offsets within the compiled regex. The default is 2, which allows for
   compiled patterns up to 64K long. This covers the vast majority of cases.
   However, PCRE2 can also be compiled to use 3 or 4 bytes instead. This
   allows for longer patterns in extreme cases. */
#ifndef LINK_SIZE
#define LINK_SIZE 2
#endif

/* Define to the sub-directory where libtool stores uninstalled libraries. */
/* This is ignored unless you are using libtool. */
#ifndef LT_OBJDIR
#define LT_OBJDIR ".libs/"
#endif

/* The value of MATCH_LIMIT determines the default number of times the
   internal match() function can be called during a single execution of
   pcre2_match(). There is a runtime interface for setting a different limit.
   The limit exists in order to catch runaway regular expressions that take
   for ever to determine that they do not match. The default is set very large
   so that it does not accidentally catch legitimate cases. */
#ifndef MATCH_LIMIT
#define MATCH_LIMIT 10000000
#endif

/* The above limit applies to all calls of match(), whether or not they
   increase the recursion depth. In some environments it is desirable to limit
   the depth of recursive calls of match() more strictly, in order to restrict
   the maximum amount of stack (or heap, if HEAP_MATCH_RECURSE is defined)
   that is used. The value of MATCH_LIMIT_RECURSION applies only to recursive
   calls of match(). To have any useful effect, it must be less than the value
   of MATCH_LIMIT. The default is to use the same value as MATCH_LIMIT. There
   is a runtime method for setting a different limit. */
#ifndef MATCH_LIMIT_RECURSION
#define MATCH_LIMIT_RECURSION MATCH_LIMIT
#endif

/* This limit is parameterized just in case anybody ever wants to change it.
   Care must be taken if it is increased, because it guards against integer
   overflow caused by enormously large patterns. */
#ifndef MAX_NAME_COUNT
#define MAX_NAME_COUNT 10000
#endif

/* This limit is parameterized just in case anybody ever wants to change it.
   Care must be taken if it is increased, because it guards against integer
   overflow caused by enormously large patterns. */
#ifndef MAX_NAME_SIZE
#define MAX_NAME_SIZE 32
#endif

/* Defining NEVER_BACKSLASH_C locks out the use of \C in all patterns. */
/* #undef NEVER_BACKSLASH_C */

/* The value of NEWLINE_DEFAULT determines the default newline character
   sequence. PCRE2 client programs can override this by selecting other values
   at run time. The valid values are 1 (CR), 2 (LF), 3 (CRLF), 4 (ANY), and 5
   (ANYCRLF). */
#ifndef NEWLINE_DEFAULT
#define NEWLINE_DEFAULT 2
#endif

/* Name of package */
#define PACKAGE "pcre2"

/* Define to the address where bug reports for this package should be sent. */
#define PACKAGE_BUGREPORT ""

/* Define to the full name of this package. */
#define PACKAGE_NAME "PCRE2"

/* Define to the full name and version of this package. */
#define PACKAGE_STRING "PCRE2 10.22"

/* Define to the one symbol short name of this package. */
#define PACKAGE_TARNAME "pcre2"

/* Define to the home page for this package. */
#define PACKAGE_URL ""

/* Define to the version of this package. */
#define PACKAGE_VERSION "10.22"

/* The value of PARENS_NEST_LIMIT specifies the maximum depth of nested
   parentheses (of any kind) in a pattern. This limits the amount of system
   stack that is used while compiling a pattern. */
#ifndef PARENS_NEST_LIMIT
#define PARENS_NEST_LIMIT 250
#endif

/* The value of PCRE2GREP_BUFSIZE determines the size of buffer used by
   pcre2grep to hold parts of the file it is searching. This is also the
   minimum value. The actual amount of memory used by pcre2grep is three times
   this number, because it allows for the buffering of "before" and "after"
   lines. */
#ifndef PCRE2GREP_BUFSIZE
#define PCRE2GREP_BUFSIZE 20480
#endif

/* Define to any value to include debugging code. */
/* #undef PCRE2_DEBUG */

/* If you are compiling for a system other than a Unix-like system or
   Win32, and it needs some magic to be inserted before the definition
   of a function that is exported by the library, define this macro to
   contain the relevant magic. If you do not define this macro, a suitable
    __declspec value is used for Windows systems; in other environments
   "extern" is used for a C compiler and "extern C" for a C++ compiler.
   This macro apears at the start of every exported function that is part
   of the external API. It does not appear on functions that are "external"
   in the C sense, but which are internal to the library. */
/* #undef PCRE2_EXP_DEFN */

/* Define to any value if linking statically (TODO: make nice with Libtool) */
/* #undef PCRE2_STATIC */

/* Define to necessary symbol if this constant uses a non-standard name on
   your system. */
/* #undef PTHREAD_CREATE_JOINABLE */

/* Define to 1 if you have the ANSI C header files. */
/* #undef STDC_HEADERS */

/* Define to any value to enable support for Just-In-Time compiling. */
/* #undef SUPPORT_JIT */

/* Define to any value to allow pcre2grep to be linked with libbz2, so that it
   is able to handle .bz2 files. */
/* #undef SUPPORT_LIBBZ2 */

/* Define to any value to allow pcre2test to be linked with libedit. */
/* #undef SUPPORT_LIBEDIT */

/* Define to any value to allow pcre2test to be linked with libreadline. */
/* #undef SUPPORT_LIBREADLINE */

/* Define to any value to allow pcre2grep to be linked with libz, so that it
   is able to handle .gz files. */
/* #undef SUPPORT_LIBZ */

/* Define to any value to enable callout script support in pcre2grep. */
/* #undef SUPPORT_PCRE2GREP_CALLOUT */

/* Define to any value to enable JIT support in pcre2grep. */
/* #undef SUPPORT_PCRE2GREP_JIT */

/* Define to any value to enable the 16 bit PCRE2 library. */
/* #undef SUPPORT_PCRE2_16 */

/* Define to any value to enable the 32 bit PCRE2 library. */
/* #undef SUPPORT_PCRE2_32 */

/* Define to any value to enable the 8 bit PCRE2 library. */
/* #undef SUPPORT_PCRE2_8 */

/* Define to any value to enable support for Unicode and UTF encoding. This
   will work even in an EBCDIC environment, but it is incompatible with the
   EBCDIC macro. That is, PCRE2 can support *either* EBCDIC code *or*
   ASCII/Unicode, but not both at once. */
/* #undef SUPPORT_UNICODE */
#define SUPPORT_UNICODE

/* Define to any value for valgrind support to find invalid memory reads. */
/* #undef SUPPORT_VALGRIND */

/* Version number of package */
#define VERSION "10.22"

/* Define to empty if `const' does not conform to ANSI C. */
/* #undef const */

/* Define to the type of a signed integer type of width exactly 64 bits if
   such a type exists and the standard includes do not define it. */
/* #undef int64_t */

/* Define to `unsigned int' if <sys/types.h> does not define. */
/* #undef size_t */
