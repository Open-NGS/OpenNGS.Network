using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData
{
    public class ModelData
    {
        public int TransformID { get; set; }
        public List<string> ChangeComponents { get; set; }
        public string ID { get; set; }
        public string Resource { get; set; }
        public float Scale { get; set; }
        public bool ScaleXInverse { get; set; }
        public bool CloseLOD { get; set; }
    }
}
