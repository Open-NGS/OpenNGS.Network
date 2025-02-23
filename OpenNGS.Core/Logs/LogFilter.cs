using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Logs
{

    public class LogFilter
    {
        public enum FilterType
        {
            /// <summary>
            /// -1 : Filter All(Log all logs)
            /// </summary>
            All = -1,
            /// <summary>
            /// 0 : Filter Nothing(No log will be logged)
            /// </summary>
            None = 0,
            /// <summary>
            /// 1 : Filter Error(Error log will be logged)
            /// </summary>
            Error = 1 << LogType.Error,
            /// <summary>
            /// 2 : Filter Error(Error log will be logged)
            /// </summary>
            Log = 1 << LogType.Log,
            /// <summary>
            /// 4 : Filter Error(Error log will be logged)
            /// </summary>
            Warning = 1 << LogType.Warning,
            /// <summary>
            /// 8 : Filter Error(Error log will be logged)
            /// </summary>
            Exception = 1 << LogType.Exception,
            /// <summary>
            /// 16 : Filter Error(Error log will be logged)
            /// </summary>
            Assert = 1 << LogType.Assert,

        }

        class TypeFilter
        {
            private int filter;

            public TypeFilter(FilterType filter)
            {
                this.filter = (int)filter;
            }

            public bool IsFiltered(LogType type)
            {
                if (filter == (int)FilterType.All)
                    return true;
                if (filter == (int)FilterType.None)
                    return false;

                return (filter & (1<<(int)type)) == (1 << (int)type);
            }
        }

        class TagFilter
        {
            static readonly char[] spliter = new char[] { '|' };
            HashSet<string> Tags = new HashSet<string>();
            public TagFilter(string tags)
            {
                if (string.IsNullOrEmpty(tags))
                    return;
                string[] allTags = tags.ToLower().Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                if(allTags.Contains("all"))
                    return;
                foreach(string tag in allTags)
                {
                    Tags.Add(tag);
                }
            }

            public bool IsFiltered(string tag)
            {
                if (this.Tags.Count == 0)
                    return true;

                if (string.IsNullOrEmpty(tag))
                    return false;

                return this.Tags.Contains(tag.ToLower());
            }
        }

        TypeFilter typeFilter;
        TagFilter tagFilter;

        public LogFilter(FilterType filter,string tags)
        {
            typeFilter = new TypeFilter(filter);
            tagFilter = new TagFilter(tags);
        }

        public bool IsFiltered(LogType type,string tag)
        {
            if (!typeFilter.IsFiltered(type))
                return false;

            return tagFilter.IsFiltered(tag);
        }
    }
}
