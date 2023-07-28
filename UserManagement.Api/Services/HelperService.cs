namespace UserManagement.Api.Services;

public class HelperService : IHelperService
{
    private readonly List<string>? _roles;
    public HelperService(IConfiguration configuration)
    {
        _roles = configuration.GetSection("Roles").Get<List<string>>();
    }

    public bool CompareRights(List<string> role1, List<string>? role2)
    {
        if (_roles == null)
        {
            throw new Exception("Role must have value");
        }
        // check same list roles
        if (role2 != null)
        {
            var firstNotSecond = role1.Except(role2).ToList();
            var secondNotFirst = role2.Except(role1).ToList();
            if (!firstNotSecond.Any() && !secondNotFirst.Any())
            {
                return true;
            }
        }

        // check contain in list roles
        if (role1.Intersect(_roles).Count() != role1.Count || role2?.Intersect(_roles).Count() != role2?.Count)
        {
            throw new Exception("Roles must contain");
        }
        // check level of role
        var res = true;
        for (var i = 0; i < role1.Count; i++)
        {
            var allRes = role2?.Where((_, j) => _roles.IndexOf(role1.ElementAt(i)) > _roles.IndexOf(role2.ElementAt(j))).Count();

            res = allRes == 0;
            if(res) return true;
        }
        
        return res;
    }
}
