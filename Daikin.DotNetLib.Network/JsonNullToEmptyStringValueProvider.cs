using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Daikin.DotNetLib.Network
{
    public class JsonNullToEmptyStringValueProvider : IValueProvider
    {
        #region Fields
        private readonly PropertyInfo _memberInfo;
        #endregion

        #region Constructors
        public JsonNullToEmptyStringValueProvider(PropertyInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }
        #endregion

        #region Methods
        public object GetValue(object target)
        {
            var result = _memberInfo.GetValue(target);
            if (_memberInfo.PropertyType == typeof(string) && result == null) result = "";
            return result;

        }

        public void SetValue(object target, object value)
        {
            _memberInfo.SetValue(target, value);
        }
        #endregion
    }
}
