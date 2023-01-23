namespace GardenLogWeb.Pages.Harvest.Components
{
    public class PlantHarvestFilter : Base
    {
        public string? PlantId { get; set; }

        public bool IsStartIndoors { get; set; }
        public bool IsDirectSow { get; set; }
    }

    public class Base
    {
        private Dictionary<String, Dictionary<string, string>> _errors = new Dictionary<String, Dictionary<string, string>>();
        public event EventHandler<EventArgs> ModelChanged;

        public int Id { get; set; }

        #region "Public Fucntions"
        public String GetValue(String fieldName)
        {
            var propertyInfo = this.GetType().GetProperty(fieldName);
            var value = propertyInfo.GetValue(this);

            if (value != null) { return value.ToString(); }
            return String.Empty;
        }
        public void SetValue(String fieldName, object value)
        {
            var propertyInfo = this.GetType().GetProperty(fieldName);
            propertyInfo.SetValue(this, value);
            CheckRules(fieldName);
        }
        #endregion

        #region "Public Validation Functions"

        public bool CheckRules()
        {
            foreach (var propInfo in this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                CheckRules(propInfo.Name);

            return HasErrors();
        }

        public void CheckRules(String fieldName)
        {
            var propertyInfo = this.GetType().GetProperty(fieldName);
            var attrInfos = propertyInfo.GetCustomAttributes(true);
            foreach (var attrInfo in attrInfos)
            {
                if (attrInfo is IModelRule modelrule)
                {
                    var value = propertyInfo.GetValue(this);
                    var result = modelrule.Validate(fieldName, value);
                    if (result.IsValid)
                    {
                        RemoveError(fieldName, attrInfo.GetType().Name);
                    }
                    else
                    {
                        AddError(fieldName, attrInfo.GetType().Name, result.Message);
                    }
                }
            }
            OnModelChanged();
        }

        public String Errors(String fieldName)
        {
            if (!_errors.ContainsKey(fieldName)) { _errors.Add(fieldName, new Dictionary<string, string>()); }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var value in _errors[fieldName].Values)
                sb.AppendLine(value);

            return sb.ToString();
        }

        public bool HasErrors()
        {
            int errorcount = 0;
            foreach (var key in _errors.Keys)
                errorcount += _errors[key].Keys.Count;
            return (errorcount != 0);
        }
        public bool HasErrors(String fieldName)
        {
            if (!_errors.ContainsKey(fieldName)) { _errors.Add(fieldName, new Dictionary<string, string>()); }
            return (_errors[fieldName].Values.Count > 0);
        }


        #endregion

        #region "Private Functions"
        private void AddError(String fieldName, String ruleName, String errorText)
        {
            if (!_errors.ContainsKey(fieldName)) { _errors.Add(fieldName, new Dictionary<string, string>()); }
            if (_errors[fieldName].ContainsKey(ruleName)) { _errors[fieldName].Remove(ruleName); }
            _errors[fieldName].Add(ruleName, errorText);
            OnModelChanged();
        }

        private void RemoveError(String fieldName, String ruleName)
        {
            if (!_errors.ContainsKey(fieldName)) { _errors.Add(fieldName, new Dictionary<string, string>()); }
            if (_errors[fieldName].ContainsKey(ruleName))
            {
                _errors[fieldName].Remove(ruleName);
                OnModelChanged();
            }
        }
        #endregion

        protected void OnModelChanged()
        {
            ModelChanged?.Invoke(this, new EventArgs());
        }



    }

    public interface IModelRule
    {
        ValidationResult Validate(String fieldName, object fieldValue);
    }
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public String Message { get; set; }
    }
}
