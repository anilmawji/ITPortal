using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation.Parameter;

// Extends from IEnumerable instead of List<PSParameter> to force caller to interact with the list through exposed methods only
public class PSParameterList : IEnumerable<PSParameter>
{
    private readonly List<PSParameter> _parameters = new();

    public PSParameterList() { }

    public PSParameterList(IEnumerable<ParameterAst> parameters)
    {
        foreach (var parameter in parameters)
        {
            Add(parameter);
        }
    }

    public void Add(ParameterAst parameter)
    {
        _parameters.Add(new PSParameter(
            parameter.Name.VariablePath.ToString(),
            parameter.StaticType,
            parameter.IsMandatory()
        ));
    }

    public void Add(string parameterName, Type parameterType, bool mandatory = false)
    {
        _parameters.Add(new PSParameter(parameterName, parameterType, mandatory));
    }

    public void Register(PowerShell shell)
    {
        foreach (var parameter in _parameters)
        {
            shell.AddParameter(parameter.Name, parameter.Value);
        }
    }

    public IEnumerator<PSParameter> GetEnumerator()
    {
        return _parameters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _parameters.GetEnumerator();
    }
}
