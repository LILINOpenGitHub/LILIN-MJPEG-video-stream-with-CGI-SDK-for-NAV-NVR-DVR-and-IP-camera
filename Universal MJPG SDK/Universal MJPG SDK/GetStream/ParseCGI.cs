using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GetStream
{
    public class ParseCGI
    {
        public String Content { get; set; }
        public String KeyValueSeparator { get; set; }
        public String CommandSeparator { get; set; }
        public List<String> CommandList { get; set; }
        public SortedList<String, int> Commands { get; set; }
        public SortedList<String, String> Commands2 { get; set; }

        public ParseCGI()
        {
            this.CommandList = new List<string>();
            this.Commands = new SortedList<string, int>();
            this.Commands2 = new SortedList<string, string>();
        }

        public void Parse(String content)
        {
            if (content == null)
            {
                return;
            }
            
            String[] sArray = Regex.Split(content, CommandSeparator, RegexOptions.IgnoreCase);
            CommandList.Clear();
            Commands.Clear();
            Commands2.Clear();
            foreach (String _command in sArray)
            {
                CommandList.Add(_command);
                String[] keyValue = Regex.Split(_command, KeyValueSeparator, RegexOptions.IgnoreCase);
                if (keyValue.Length == 2)
                {
                    //Commands.Add(keyValue[0], Int32.Parse(keyValue[1]));
                    Commands2.Add(keyValue[0], keyValue[1]);
                }
            }
        }

        /// <summary>
        /// 從字串中取值
        ///     ex: content: "keyword=value"
        ///     則  key: "keyword"
        ///         separator: "="
        /// </summary>
        /// <param name="content">字串內容</param>
        /// <param name="key">關鍵字</param>
        /// <param name="separator">分隔符號</param>
        /// <returns>拆解後的資料</returns>
        public String GetValue(String content, String key, String separator)
        {
            // 解析結果
            String value = null;

            // 關鍵字 在 字串內容 的 起始位置
            int keyPos = content.IndexOf(key);
            // 如果 位置 小於 0
            // 表示 關鍵字 不存在
            if (keyPos < 0)
            {
                // 結束
                return null;
            }

            // 分隔符號 在 字串內容 的 起始位置
            int sepPos = content.IndexOf(separator);
            // 如果 位置 小於 0
            // 表示 分隔符號 不存在
            if (sepPos < 0)
            {
                // 結束
                return null;
            }

            // 解析結果
            // 從 字串內容 中 切出 位置 從 分隔符號 後一位 到 結尾位置
            value = content.Substring(sepPos + 1);

            // 回傳 結果
            return value;
        }

    }
}
