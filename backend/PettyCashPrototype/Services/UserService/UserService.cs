﻿using AutoMapper;
using PettyCashPrototype.Mappers.UserMapper;

namespace PettyCashPrototype.Services.UserService
{
    public class UserService: IUser
    {
        private readonly PettyCashPrototypeContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public UserService(PettyCashPrototypeContext db, UserManager<User> userManager, IMapper mapper) 
        {
            _userManager = userManager;
            _db = db; 
            _mapper = mapper;
        }


        public async Task<IEnumerable<UserMapper>> GetAll()
        {
            try
            {
                IEnumerable<User> users = await _db.Users
                    .Where(a => a.IsActive == true)
                    .ToListAsync();

                IEnumerable<UserMapper> userMapped =  (users
                    .Select(c => _mapper.Map<UserMapper>(c))
                    .ToList());

                if (userMapped == null)
                    throw new Exception("System could not find any users.");

                return userMapped;
            }
            catch { throw; }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                User user = await _db.Users
                    .Where(a => a.IsActive == true)
                    .SingleAsync(e  => e.Email == email);

                if (user == null) throw new Exception("System was not able to retrieve user");

                return user;
            }
            catch { throw; }
        }

        public async Task<UserMapper> GetMappedUserByEmail(string email)
        {
            try
            {
                User user = await _db.Users
                    .Where(a => a.IsActive == true)
                    .SingleAsync(e => e.Email == email);

                IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
                
                UserMapper userMapper = _mapper.Map<UserMapper>(user);
                userMapper.Role = roles.Single();

                return userMapper;
            } catch { throw; }
        }

        public async Task<string> Create(User user, string password)
        {
            try
            {
                IdentityResult result = new IdentityResult();
                User registeredUser = await GetUserByEmail(user.Email!);

                if (registeredUser == null)
                {
                    result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        await _db.SaveChangesAsync();
                        return user.Id;
                    }
                    else
                        throw new DBConcurrencyException($"System could not save user: {new { errors = result.Errors }}");
                }
                else
                    throw new Exception("The credentials entered in already belong to another user.");
            }
            catch { throw; }
        }
    }
}