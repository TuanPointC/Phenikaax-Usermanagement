using AutoMapper;
using UserManagement.Api.Repositorys;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepo _usersRepo;
    private readonly IMapper _mapper;

    public UsersService(IUsersRepo usersRepo, IMapper mapper)
    {
        _usersRepo = usersRepo;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users =  await _usersRepo.GetAllUsersAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<PagedList<UserDto>> GetUsersAsync(UserParams userParams)
    {
        var pagedList = await _usersRepo.GetUsersAsync(userParams);
        return pagedList;
    }
    

    public async Task<UserDto?> GetUsersByIdAsync(Guid id)
    {
        var user = await  _usersRepo.GetUsersByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<User?> GetUserNotDtoByIdAsync(Guid id)
    {
        var user = await  _usersRepo.GetUsersByIdAsync(id);
        return user;
    }

    public async Task<UserDto?> CreateUserAsync(UserDto user, string password)  
    {
        var userDb = _mapper.Map<User>(user);
        userDb.Password = BCrypt.Net.BCrypt.HashPassword(password);
        await _usersRepo.CreateUserAsync(userDb);
        return user;
    }

    public async Task<UserDto?> UpdateUserAsync(UserDto user)
    {
        var currentUser = await _usersRepo.GetUsersByIdAsync(user.Id);
        var mappedUser = _mapper.Map<UserDto, User>(user);
        if (currentUser == null) throw new Exception("Couldn't update user'");
        mappedUser.Password = currentUser.Password;
        await _usersRepo.UpdateUserAsync(mappedUser);
        return _mapper.Map<UserDto>(currentUser);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await _usersRepo.DeleteUserAsync(id);
    }
}
