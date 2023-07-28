using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Repositorys;

public class UsersRepo : IUsersRepo
{
    private readonly UsersDbContext _userDbContext;
    private readonly IMapper _mapper;

    public UsersRepo(UsersDbContext userDbContext, IMapper mapper)
    {
        _userDbContext = userDbContext;
        _mapper = mapper;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userDbContext.Users.Include(u => u.Roles).ToListAsync();
    }

    public async Task<PagedList<UserDto>> GetUsersAsync(UserParams userParams)
    {
        var query =  _userDbContext.Users.Where(u =>u.UserName != null && u.UserName.Contains(userParams.SearchName?? "")).Include(u => u.Roles).AsQueryable();
        return await PagedList<UserDto>.CreateAsync(query.ProjectTo<UserDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);
    }

    public async Task<User?> GetUsersByIdAsync(Guid id)
    {
        return await _userDbContext.Users.Include(r=>r.Roles).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> CreateUserAsync(User? user)
    {
        if (user == null) throw new NullReferenceException("User");
        await _userDbContext.Users.AddAsync(user);
        await _userDbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        var currentUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (currentUser == null) return currentUser;
        currentUser.UserName = user.UserName;
        currentUser.Roles = user.Roles;
        await _userDbContext.SaveChangesAsync();
        return currentUser;
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var currentUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (currentUser != null)
        {
            _userDbContext.Users.Remove(currentUser);
            await _userDbContext.SaveChangesAsync();
        }
    }
}
