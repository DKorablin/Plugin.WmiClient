
/*
@echo off && cls
set WinDirNet=%WinDir%\Microsoft.NET\Framework
IF EXIST "%WinDirNet%\v2.0.50727\csc.exe" set csc="%WinDirNet%\v2.0.50727\csc.exe"
IF EXIST "%WinDirNet%\v3.5\csc.exe" set csc="%WinDirNet%\v3.5\csc.exe"
IF EXIST "%WinDirNet%\v4.0.30319\csc.exe" set csc="%WinDirNet%\v4.0.30319\csc.exe"
%csc% /nologo /reference:"C:\Visual Studio Projects\ASP.NET\ws.Cdek.Exist.ru\ws.Cdek.Exist.ru\bin\Exist.Common.dll" /reference:"C:\Visual Studio Projects\ASP.NET\ws.Cdek.Exist.ru\ws.Cdek.Exist.ru\bin\Exist.Dal.Common.dll" /reference:"C:\Visual Studio Projects\ASP.NET\ws.Cdek.Exist.ru\ws.Cdek.Exist.ru\bin\Exist.Dal.Logistics.dll" /reference:"C:\Visual Studio Projects\ASP.NET\ws.Cdek.Exist.ru\ws.Cdek.Exist.ru\bin\Mindbox.Data.Linq.dll" /reference:"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Windows.Forms\Flatbed.Dialog\Flatbed.Dialog\bin\Debug\SAL.Flatbed.dll" /reference:"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Windows.Forms\Flatbed.Dialog\Flatbed.Dialog\bin\Debug\SAL.Windows.dll" /out:"%~0.exe" %0
"%~0.exe"
del "%~0.exe"
PAUSE
exit
*/
using Exist.Dal.Logistics;
using SAL.Flatbed;
using SAL.Windows;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Management;


namespace Plugin.Compiler.Runtime
{
	public class Undefined
	{
		public void PluginMain(params Object[] args)
		{
// Method Begin
Int32 x  = 2;
Int32 y  = 0;
Single a = checked((Single)x/(Single)y);
MessageBox.Show(a.ToString());
// Method End
		}
	}
}
class Program
{
	static void Main(String[] args) { new Plugin.Compiler.Runtime.Undefined().PluginMain(args); }
}