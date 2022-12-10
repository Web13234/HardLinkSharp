using System;
using System.Runtime.InteropServices;

namespace HardLinkSharp;

public static class HardLink
{
    private static class Import
    {
        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        internal extern static bool CreateHardLink(string linkName, string sourceName, IntPtr attribute);
    }

    public static bool TryCreateHardLink(string sourceName, string linkName, out string ErrorInformation)
    {
        ErrorInformation = "UnknownError";

        if (sourceName[0] != linkName[0])
        {
            ErrorInformation = "Each link to the same file must be on the same drive";
            return false;
        }
        if(new DriveInfo(sourceName.Remove(1)).DriveFormat!="NTFS")
        {
            ErrorInformation = "Hardlink can only be created in NTFS drive";
            return false;
        }
        if(!File.Exists(sourceName))
        {
            ErrorInformation = "Scource file does not exist";
            return false;
        }
        if(File.Exists(linkName))
        {
            ErrorInformation = "Target file is already exists";
            return false;
        }

        return Import.CreateHardLink(linkName, sourceName, IntPtr.Zero);
    }
    public static void CreateHardLink(string sourceName, string linkName)
    {
        if (!TryCreateHardLink(sourceName, linkName, out string error))
        {
            throw new HardlinkCreatException(error);
        }
    }


}

public class HardlinkCreatException : Exception
{
    public HardlinkCreatException() { }
    public HardlinkCreatException(string message) : base(message) { }
}
