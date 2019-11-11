using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setup_Allegro {
    static class PatchContext {
        public static string gccPatch = @"
			<gcc>
				<NAME>
					<str>
						<![CDATA[GNU GCC Compiler]]>
					</str>
				</NAME>
				<LINKER_OPTIONS>
					<str>
						<![CDATA[-static;]]>
					</str>
				</LINKER_OPTIONS>
				<INCLUDE_DIRS>
					<str>
						<![CDATA[C:\API\Allegro_5.2.5.1\include;]]>
					</str>
				</INCLUDE_DIRS>
				<LIBRARY_DIRS>
					<str>
						<![CDATA[C:\API\Allegro_5.2.5.1\lib;]]>
					</str>
				</LIBRARY_DIRS>
				<LIBRARIES>
					<str>
						<![CDATA[liballegro_monolith-static.a;libdumb.a;libFLAC.a;libfreetype.a;libjpeg.a;libopusfile.a;libopus.a;libphysfs.a;libpng16.a;libtheoradec.a;libvorbisfile.a;libvorbis.a;libogg.a;libwebp.a;libwebpdecoder.a;libwebpdemux.a;libzlib.a;libpsapi.a;libwinmm.a;libshlwapi.a;libgdi32.a;libopengl32.a;libole32.a;libdsound.a;libpthread.a;libkernel32.a;libuser32.a;libwinspool.a;libshell32.a;liboleaut32.a;libuuid.a;libcomdlg32.a;libadvapi32.a;]]>
					</str>
				</LIBRARIES>
				<MASTER_PATH>
					<str>
						<![CDATA[C:\Compiler\MinGW_8.2.1]]>
					</str>
				</MASTER_PATH>
				<C_COMPILER>
					<str>
						<![CDATA[gcc.exe]]>
					</str>
				</C_COMPILER>
				<CPP_COMPILER>
					<str>
						<![CDATA[g++.exe]]>
					</str>
				</CPP_COMPILER>
				<LINKER>
					<str>
						<![CDATA[g++.exe]]>
					</str>
				</LINKER>
				<DEBUGGER_CONFIG>
					<str>
						<![CDATA[gdb_debugger:Default]]>
					</str>
				</DEBUGGER_CONFIG>
			</gcc>
";

        public static string gdbPatch = @"
				<conf1>
					<NAME>
						<str>
							<![CDATA[Default]]>
						</str>
					</NAME>
					<values>
						<EXECUTABLE_PATH>
							<str>
								<![CDATA[C:\Compiler\MinGW_8.2.1\bin\gdb.exe]]>
							</str>
						</EXECUTABLE_PATH>
						<DISABLE_INIT bool=""1"" />
						<USER_ARGUMENTS>
							<str>
								<![CDATA[]]>
							</str>
						</USER_ARGUMENTS>
						<TYPE int=""0"" />
						<INIT_COMMANDS>
							<str>
								<![CDATA[]]>
							</str>
						</INIT_COMMANDS>
						<WATCH_ARGS bool=""1"" />
						<WATCH_LOCALS bool=""1"" />
						<CATCH_EXCEPTIONS bool=""1"" />
						<EVAL_TOOLTIP bool=""0"" />
						<ADD_OTHER_SEARCH_DIRS bool=""0"" />
						<DO_NOT_RUN bool=""0"" />
						<DISASSEMBLY_FLAVOR int=""0"" />
						<INSTRUCTION_SET>
							<str>
								<![CDATA[]]>
							</str>
						</INSTRUCTION_SET>
					</values>
				</conf1>
";
    }
}
