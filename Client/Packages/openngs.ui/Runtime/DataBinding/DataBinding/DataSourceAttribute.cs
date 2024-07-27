using System;

namespace OpenNGS.UI.DataBinding
{
    public class DataSourceAttribute : Attribute
    {
        private readonly Type m_ViewModelType;
        
        public DataSourceAttribute(Type type)
        {
            m_ViewModelType = type;
        }

        public Type GeViewModelType()
        {
            return m_ViewModelType;
        }
    }
}