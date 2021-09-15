using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using CSharpHelpers;
using System.Buffers;

namespace EG
{

    public static class FileUtility
    {

        public static string CheckInvalidFileName( string filePath )
        {
            string fileName = filePath;
            string directoryName = "";
            filePath = filePath.Replace( '\\', '/' );
            int n = filePath.LastIndexOf( "/" );
            if( n != -1 )
            {
                directoryName = filePath.Substring( 0, n + 1 );
                fileName = filePath.Substring( n+1, filePath.Length-(n+1) );
            }
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            if( fileName.IndexOfAny( invalidChars ) != -1 )
            {
                for( int i = 0; i < invalidChars.Length; ++i )
                {
                    fileName = fileName.Replace( invalidChars[i], 'X' );
                }
                filePath = directoryName + fileName;
            }
            return filePath;
        }
        
        public static void CreateDirectory( string path )
        {
            path = System.IO.Path.GetDirectoryName( path );
            
            path = path.Replace( '\\', '/' );
            string[]    folders = path.Split( '/' );
            string      fpath   = "";
            
            for( int i = 1; i < folders.Length; ++i )
            {
                fpath += folders[i-1];
                if( System.IO.Directory.Exists( fpath + "/" + folders[i] ) == false )
                {
                    // 
                    System.IO.Directory.CreateDirectory( fpath + "/" + folders[i] );
                }
                fpath += "/";
            }
        }
        
        public static byte[] Read( string filePath )
        {
            // 
            if( System.IO.File.Exists( filePath ) == false )
            {
                Debug.LogWarning( "file not found -> " + filePath );
                return null;
            }
            
            // 
            System.IO.FileStream sr = System.IO.File.OpenRead( filePath );
            byte[] byteArray = new byte[ sr.Length ];
            sr.Read( byteArray, 0, (int)sr.Length );
            sr.Close();
            
            return byteArray;
        }
        public static string ReadToString( string filePath )
        {
            // 
            if( System.IO.File.Exists( filePath ) == false )
            {
                Debug.LogWarning( "file not found -> " + filePath );
                return null;
            }
            
            string data = "";
            StreamReader sr = new StreamReader( filePath, System.Text.Encoding.UTF8 );
            data = sr.ReadToEnd();
            sr.Close();
            
            return data;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 書き込み
        /// </summary>
        /// ***********************************************************************
        public static void Write( string filePath, byte[] data )
        {
            if( System.IO.File.Exists( filePath ) == false )
            {
                FileUtility.MakeDirectory( filePath );
            }
            // 書き込み
            System.IO.File.WriteAllBytes( filePath, data );
        }
        public static void Write( string filePath, byte[] header, byte[] data )
        {
            filePath = CheckInvalidFileName( filePath );
            FileStream stream = System.IO.File.OpenWrite( filePath );
            stream.Write( header, 0, header.Length );
            stream.Write( data, 0, data.Length );
            stream.Close();
        }
        public static void Write( string filePath, string data )
        {
            filePath = CheckInvalidFileName( filePath );

            if (System.IO.File.Exists(filePath) == false)
            {
                FileUtility.MakeDirectory(filePath);
            }
            StreamWriter sw = new StreamWriter( filePath, false, System.Text.Encoding.UTF8 );
            data = data.Replace( "\r\n", "\n" );
            sw.NewLine = "\n";
            sw.WriteLine( data );
            sw.Flush();
            sw.Close();
        }
        
        //=========================================================================
        //. ヘルパー
        //=========================================================================
        
        /// ***********************************************************************
        /// <summary>
        /// フォルダ生成
        /// </summary>
        /// ***********************************************************************
        public static void MakeDirectory( string path )
        {
            path = System.IO.Path.GetDirectoryName( path );
            
            path = path.Replace( '\\', '/' );
            string[]    folders = path.Split( '/' );
            string      fpath   = "";
            
            for( int i = 1; i < folders.Length; ++i )
            {
                fpath += folders[i-1];
                if( System.IO.Directory.Exists( fpath + "/" + folders[i] ) == false )
                {
                    // フォルダ生成
                    System.IO.Directory.CreateDirectory( fpath + "/" + folders[i] );
    //              UnityEditor.AssetDatabase.CreateFolder( fpath, folders[i] );
                }
                fpath += "/";
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// フォルダサイズを取得
        /// </summary>
        /// ***********************************************************************
        public static long GetDirectorySize( DirectoryInfo info )
        {
            long size = 0;
            
            //フォルダ内の全ファイルの合計サイズを計算する
            foreach( FileInfo fi in info.GetFiles() )
                size += fi.Length;
            
            //サブフォルダのサイズを合計していく
            foreach( DirectoryInfo di in info.GetDirectories() )
                size += GetDirectorySize(di);
            
            //結果を返す
            return size;
        }

        public static long GetDirectorySize( string path )
        {
            return GetDirectorySize( new System.IO.DirectoryInfo( path ) );
        }

        public static string ReadSingleLine( string path, int lineIndex )
        {
            using (var fs = File.OpenRead(path))
            {
                var pos = 0L;
                var i = -1;
                var b = 0;
                while ((b = fs.ReadByte()) > -1)
                {
                    if (b == (byte)'\n')
                    {
                        if (++i == lineIndex)
                        {
                            break;
                        }
                        else
                        {
                            pos = fs.Position;
                        }
                    }
                }
                if (b < 0) return null;

                var len = fs.Position - pos - 1;
                if (len > 0xffff) return null;

                var count = (int)len;
                fs.Position = pos;
                var bytes = ArrayPool<byte>.Shared.Rent(count);
                fs.Read(bytes, 0, count);
                var s = System.Text.Encoding.UTF8.GetString(bytes, 0, count);
                ArrayPool<byte>.Shared.Return(bytes);


                return s;
            }
        }
    }
}
