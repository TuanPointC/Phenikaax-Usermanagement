namespace UserManagement.Api.Services;

public interface IHelperService
{
    public bool CompareRights(List<string> role1, List<string>? role2);
}
