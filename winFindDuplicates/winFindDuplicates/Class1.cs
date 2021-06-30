using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class MD5
{
    public long LengthOfFile;
    public long Current;
    public string FileName;

    public string Hash;
    public void GetHash()
    {
        MemoryStream ms = new MemoryStream();
        MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();
        FileStream fs = new FileStream(FileName, FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        byte[] buffer = null;
        CryptoStream cs = new CryptoStream(ms, md5Hash, CryptoStreamMode.Write);

        LengthOfFile = fs.Length;

        buffer = br.ReadBytes(10000);
        while (buffer.Length != 0)
        {
            cs.Write(buffer, 0, buffer.Length);
            Current = fs.Position;
            buffer = br.ReadBytes(10000);
        }

        cs.FlushFinalBlock();
        br.Close();
        fs.Close();

        byte[] hash = md5Hash.Hash;

        StringBuilder buff = new StringBuilder();
        byte hashByte = 0;
        foreach (byte hashByte_loopVariable in hash)
        {
            hashByte = hashByte_loopVariable;
            buff.Append(string.Format("{0:X1}", hashByte));
        }

        this.Hash = buff.ToString();
        Current = -1;
    }

    public MD5(string FileName)
    {
        this.FileName = FileName;
        Current = 0;
    }
}
