using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace OpenNGS.Logs.Appenders
{
    /// <summary>
    /// File appender
    /// </summary>
    public class FileAppender : BaseAppender
    {
        public override string TypeIdentify => "File";

        private StreamWriter fileWriter;
        private string filename;
        private string filePath = "";

        public FileAppender(string name) : base(name)
        {
        }

        public override void Init(AppenderConfig config)
        {
            base.Init(config);
            if (fileWriter != null)
            {
                fileWriter.Close();
                fileWriter.Dispose();
            }
            fileWriter = null;

            this.filename = config.LogFile;

            filePath = IO.FileSystem.LogPath + "/";

            if(config.Enable)
            {
                this.CreateLogFile();
            }
        }


        private void CreateLogFile()
        {
            try
            {
                if (!Config.Roll)
                {
                    File.Delete(filePath + this.filename);
                    string name = filePath + this.filename;
                    fileWriter = File.AppendText(name);
                    OpenNGSDebug.Log("CreateLogFile:" + new FileInfo(name).FullName);
                }
                else
                {
                    string name = filePath + this.filename.Replace(".", "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture) + ".");
                    File.Delete(name);
                    fileWriter = File.AppendText(name);
                    OpenNGSDebug.Log("CreateLogFile:" + new FileInfo(name).FullName);
                }

                fileWriter.AutoFlush = true;
                this.Write("******" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "******");
            }
            catch (Exception ex)
            {
                OpenNGSDebug.LogException(ex);
            }
        }

        public override void AppendFormat(string tag, LogType logType, object context, string format, params object[] args)
        {
            if(Config.Format == AppenderConfig.LogFormat.Compact)
                this.Write(string.Format(format, args));
            else
                this.Write(LogSystem.Time + "[" + tag + "][" + logType.ToString() + "]" + string.Format(format, args));
        }

        public override void AppendException(string tag, Exception exception, object context)
        {
            this.Write(LogSystem.Time + string.Format("[{0}][Exception]{1}", tag, exception.ToString()));
        }

        void Write(string content)
        {
            if (fileWriter != null && fileWriter.BaseStream != null && fileWriter.BaseStream.CanWrite)
                fileWriter.WriteLine(content);
        }
    }
}
