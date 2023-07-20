using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation;

public class PSParameterDictionary : IEnumerable
{
    private readonly Dictionary<string, PSParameter> _parameters = new();

    public void Add(ParameterAst parameter)
    {
        _parameters.Add(parameter.Name.ToString(), new PSParameter(parameter.StaticType));
    }

    public void Remove(string parameterName)
    {
        _parameters.Remove(parameterName);
    }

    public bool Update(string parameterName, object parameter)
    {
        return _parameters != null && _parameters[parameterName].SetValue(parameter);
    }

    public void Register(PowerShell shell)
    {
        if (_parameters.Any())
        {
            shell.AddParameters(_parameters);
        }
    }

    public void Clear()
    {
        _parameters.Clear();
    }

    public bool Any()
    {
        return _parameters.Any();
    }

    public IEnumerator GetEnumerator()
    {
        return _parameters.GetEnumerator();
    }
}
