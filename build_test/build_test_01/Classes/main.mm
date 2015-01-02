#include "RegisterMonoModules.h"
#include <csignal>

// Hack to work around iOS SDK 4.3 linker problem
// we need at least one __TEXT, __const section entry in main application .o files
// to get this section emitted at right time and so avoid LC_ENCRYPTION_INFO size miscalculation
static const int constsection = 0;

void UnityInitTrampoline();

// WARNING: this MUST be c decl (NSString ctor will be called after +load, so we cant really change its value)
const char* AppControllerClassName = "UnityAppController";

#if INIT_SCRIPTING_BACKEND
extern "C" void InitializeScriptingBackend();
#endif

int main(int argc, char* argv[])
{
	@autoreleasepool
	{
		UnityInitTrampoline();
		UnityParseCommandLine(argc, argv);

	#if INIT_SCRIPTING_BACKEND
		InitializeScriptingBackend();
	#endif

		RegisterMonoModules();
		NSLog(@"-> registered mono modules %p\n", &constsection);

		// iOS terminates open sockets when an application enters background mode.
		// The next write to any of such socket causes SIGPIPE signal being raised,
		// even if the request has been done from scripting side. This disables the
		// signal and allows Mono to throw a proper C# exception.
		std::signal(SIGPIPE, SIG_IGN);

		UIApplicationMain(argc, argv, nil, [NSString stringWithUTF8String:AppControllerClassName]);
	}

	return 0;
}
