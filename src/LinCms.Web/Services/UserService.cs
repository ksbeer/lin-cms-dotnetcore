﻿using AutoMapper;
using FreeSql;
using LinCms.Web.Models.Admins;
using LinCms.Web.Models.Users;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;

namespace LinCms.Web.Services
{
    public class UserService : IUserSevice
    {
        private readonly BaseRepository<LinUser> _userRepository;
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;

        public UserService(BaseRepository<LinUser> userRepository, IFreeSql freeSql, IMapper mapper)
        {
            _userRepository = userRepository;
            _freeSql = freeSql;
            _mapper = mapper;
        }

        public LinUser Authorization(string username, string password)
        {
            LinUser user = _userRepository.Select.Where(r => r.Nickname == username && r.Password == password).First();

            return user;
        }

        public bool ChangePassword(ChangePasswordDto passwordDto)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            _userRepository.Delete(r => r.Id == id);
        }

        public void ResetPassword(int id, ResetPasswordDto resetPasswordDto)
        {
            var user = _userRepository.Where(r => r.Id == id).First();

            if (user == null)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            _freeSql.Update<LinUser>(id).Set(a => new LinUser()
            {
                Password = resetPasswordDto.ConfirmPassword
            }).ExecuteAffrows();

        }

        public PagedResultDto<LinUser> GetUserList(UserSearchDto searchDto)
        {
            var linUsers = _userRepository.Select.WhereIf(searchDto.GroupId != null, r => r.GroupId == searchDto.GroupId)
                  .ToPagerList(searchDto, out long totalNums);

            return new PagedResultDto<LinUser>(linUsers, totalNums);
        }

        public void Register(LinUser user)
        {
            bool isExistGroup = _userRepository.Select.Any(r => r.Id == user.GroupId);

            if (!isExistGroup)
            {
                throw new LinCmsException("分组不存在", ErrorCode.NotFound);
            }

            var isRepeatNickName = _userRepository.Select.Any(r => r.Nickname == user.Nickname);

            if (isRepeatNickName)
            {
                throw new LinCmsException("用户名重复，请重新输入", ErrorCode.RepeatField);
            }

            if (!string.IsNullOrEmpty(user.Email.Trim()))
            {
                var isRepeatEmail = _userRepository.Select.Any(r => r.Email == user.Email.Trim());
                if (isRepeatEmail)
                {
                    throw new LinCmsException("注册邮箱重复，请重新输入", ErrorCode.RepeatField);
                }
            }

            user.Active = 1;
            user.Admin = 1;

            _userRepository.Insert(user);
        }

        /// <summary>
        /// 修改指定字段，邮件和组别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateUserDto"></param>    
        /// <returns></returns>
        public void UpdateUserInfo(int id, UpdateUserDto updateUserDto)
        {
            //此方法适用于更新字段少时
            //_freeSql.Update<LinUser>(id).Set(a => new LinUser()
            //{
            //    Email = updateUserDto.Email,
            //    GroupId = updateUserDto.GroupId
            //}).ExecuteAffrows();

            //需要多查一次
            LinUser linUser = _userRepository.Where(r => r.Id == id).ToOne();
            if (linUser == null)
            {
                throw new LinCmsException("用户不存在",ErrorCode.NotFound);
            }
            //赋值过程可使用AutoMapper简化
            //只更新 Email、GroupId
            // UPDATE `lin_user` SET `email` = ?p_0, `group_id` = ?p_1 
            // WHERE(`id` = 1) AND(`is_deleted` = 0)
            //linUser.Email = updateUserDto.Email;
            //linUser.GroupId = updateUserDto.GroupId;

            linUser = _mapper.Map<LinUser>(updateUserDto);

            linUser.Id = id;

            _userRepository.Update(linUser);

        }


        public void ChangeStatus(int id, UserActive userActive)
        {
            LinUser user = _userRepository.Select.Where(r => r.Id == id).ToOne();

            if (user==null)
            {
                throw new LinCmsException("用户不存在",ErrorCode.NotFound);
            }

            if (user.IsActive() &&userActive == UserActive.Active)
            {
                throw new LinCmsException("当前用户已处于禁止状态");
            }
            if (!user.IsActive() && userActive == UserActive.NotActive)
            {
                throw new LinCmsException("当前用户已处于激活状态");
            }

            _freeSql.Update<LinUser>(id).Set(a => new LinUser()
            {
                Active = userActive.GetHashCode()
            }).ExecuteAffrows();
        }
    }
}
