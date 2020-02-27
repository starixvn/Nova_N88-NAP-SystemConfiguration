// --------------------------------------------------------------------------
// <copyright file="UserRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.6">Created</change>
// </history>

namespace Fanex.NAP.Core.Repository.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Criteria.User;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Interface;

    /// <summary>
    /// User repository.
    /// </summary>
    public class UserRepository : BaseRepository<IUser>, IUserRepository
    {
        /// <summary>
        /// Get user information.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>User object.</returns>
        public override IUser Get(ICriteria criteria)
        {
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var objUser = objectDb.QueryEntity<User, Group, User, User, User>(
                    (u, g, uc, um) => { u.Group = g; u.Creator = uc; u.Modifier = um; return u; },
                    "GroupId,CreatorId,ModifierId",
                    criteria.GetSpParams());

                return objUser;
            }
        }

        /// <summary>
        /// Fetch user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user.</returns>
        public override IEnumerable<IUser> Fetch(ICriteria criteria)
        {
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var objListUser = objectDb.Query<User, Group, User, User, User>(
                    (u, g, uc, um) => { u.Group = g; u.Creator = uc; u.Modifier = um; return u; },
                    "GroupId,CreatorId,ModifierId",
                    criteria.GetSpParams());

                return objListUser;
            }
        }

        /// <summary>
        /// Fetch user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user.</returns>
        public IQueryable<IUser> SearchByUsername(SearchByUsernameUserCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();

                var userList = db.Query<User>(hash);

                criteria.TotalRows = Convert.ToInt32(hash["totalrows"]);

                return userList.AsQueryable();
            }
        }

        /// <summary>
        /// Insert user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Insert(InsertCustCriteria criteria, ICriteria logCriteria)
        {
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(hash);
                criteria.CustId = Convert.ToInt32(db.GetParameterValue("CustId"));

                return this.GetCommandResult(hash);
            }
        }

        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Update(ICriteria criteria, ICriteria logCriteria)
        {
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(hash);
                return this.GetCommandResult(hash);
            }
        }

        /// <summary>
        /// User check in.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void UserCheckIn(UserCheckInUserCriteria criteria)
        {
            Hashtable hash = criteria.GetSpParams();

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(hash);
                criteria.OnUserId = Convert.ToInt32(hash["onuserid"].ToString());
            }
        }

        /// <summary>
        /// User re-check in.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void UserReCheckIn(UserReCheckInUserCriteria criteria)
        {
            Hashtable hash = criteria.GetSpParams();

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(hash);
                criteria.IsKickedOut = Convert.ToBoolean(hash["iskickedout"].ToString());
            }
        }

        /// <summary>
        /// User log out.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void UserLogOut(LogOutUserCriteria criteria)
        {
            Hashtable hash = criteria.GetSpParams();

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(hash);
            }
        }

        /// <summary>
        /// User SSO check in.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void UserSSOCheckIn(UserSSOCheckInUserCriteria criteria)
        {
            Hashtable hash = criteria.GetSpParams();

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(hash);
                criteria.OnUserId = Convert.ToInt32(hash["onuserid"].ToString());
            }
        }

        /// <summary>
        /// Search user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user level.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IEnumerable<UserLevel> Search(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var userLevelList = new List<UserLevel>();

                    while (reader.Read())
                    {
                        userLevelList.Add(new UserLevel
                        {
                            Level = reader.ToInt("Level"),
                            User = reader.IsDBNull("UserId") ? null : new User
                            {
                                Id = reader.ToInt("UserId"),
                                FirstName = reader.ToString("FirstName"),
                                LastName = reader.ToString("LastName"),
                                Username = reader.ToString("Username"),
                                Email = reader.ToString("Email"),
                                Password = reader.ToString("UserPwd"),
                                LastLoginDate = reader.ToDateTime("LastLoginDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                                Group = reader.IsDBNull("GroupId") ? null : new Group
                                {
                                    Id = reader.ToShort("GroupId"),
                                    Name = reader.ToString("GroupName")
                                },
                                Creator = reader.IsDBNull("CreatorId") ? null : new User
                                {
                                    Id = reader.ToInt("CreatorId"),
                                    Username = reader.ToString("CreatorUsername")
                                },
                                Modifier = reader.IsDBNull("ModifierId") ? null : new User
                                {
                                    Id = reader.ToInt("ModifierId"),
                                    Username = reader.ToString("ModifierUsername")
                                },
                            },
                            CreatedDate = reader.ToDateTime("CreatedDateUserSite"),
                            ModifiedDate = reader.ToDateTime("ModifiedDateUserSite"),
                            Creator = reader.IsDBNull("CreatorIdUserSite") ? null : new User
                            {
                                Id = reader.ToInt("CreatorIdUserSite"),
                                Username = reader.ToString("CreatorUsernameUserSite")
                            },
                            Modifier = reader.IsDBNull("ModifierIdUserSite") ? null : new User
                            {
                                Id = reader.ToInt("ModifierIdUserSite"),
                                Username = reader.ToString("ModifierUsernameUserSite")
                            },
                            IsActive = reader.ToBool("IsActive"),
                            LastLoginDate = reader.ToDateTime("LastLoginDateUserSite")
                        });
                    }

                    return userLevelList.GroupBy(u => u.User.Id).Select(u => u.First());
                }
            }
        }

        /// <summary>
        /// Search user.
        /// </summary>
        /// <param name="criteria">The criteria search user.</param>
        /// <returns>List of user level.</returns>
        public IQueryable<UserLevel> Search(SearchUserCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var userLevelList = new List<UserLevel>();

                    // reads data
                    while (reader.Read())
                    {
                        UserLevel userLevel = ReadUserLevel(reader);

                        userLevelList.Add(userLevel);
                    }

                    reader.Close();

                    criteria.TotalRows = (int)db.GetParameterValue("totalrows");

                    return userLevelList.AsQueryable();
                }
            }
        }

        /// <summary>
        /// Get all user by username, group and site.
        /// </summary>
        /// <param name="criteria">The criteria search user.</param>
        /// <returns>List of user level.</returns>
        public IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var userLevelList = new List<UserLevel>();

                    // reads data
                    while (reader.Read())
                    {
                        UserLevel userLevel = ReadUserLevel(reader);

                        userLevelList.Add(userLevel);
                    }

                    reader.Close();

                    criteria.TotalRows = (int)db.GetParameterValue("totalrows");

                    return userLevelList.AsQueryable();
                }
            }
        }

        /// <summary>
        /// Search user exclude function.
        /// </summary>
        /// <param name="criteria">The criteria search user.</param>
        /// <returns>List of user level.</returns>
        public IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var userLevelList = new List<UserLevel>();

                    // reads data
                    while (reader.Read())
                    {
                        UserLevel userLevel = ReadUserLevel(reader);

                        userLevelList.Add(userLevel);
                    }

                    reader.Close();

                    criteria.TotalRows = (int)db.GetParameterValue("totalrows");

                    return userLevelList.AsQueryable();
                }
            }
        }

        /// <summary>
        /// Get user policy.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user policy.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IEnumerable<UserPolicy> GetUserPolicy(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var userLevelList = new List<UserPolicy>();

                    while (reader.Read())
                    {
                        userLevelList.Add(new UserPolicy
                        {
                            Policy = reader.ToInt("Policy"),
                            User = reader.IsDBNull("UserId") ? null : new User
                            {
                                Id = reader.ToInt("UserId"),
                                FirstName = reader.ToString("FirstName"),
                                LastName = reader.ToString("LastName"),
                                Username = reader.ToString("Username"),
                                Email = reader.ToString("Email"),
                                Password = reader.ToString("UserPwd"),
                                LastLoginDate = reader.ToDateTime("LastLoginDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                                Group = reader.IsDBNull("GroupId") ? null : new Group
                                {
                                    Id = reader.ToShort("GroupId"),
                                    Name = reader.ToString("GroupName")
                                },
                                Creator = reader.IsDBNull("CreatorId") ? null : new User
                                {
                                    Id = reader.ToInt("CreatorId"),
                                    Username = reader.ToString("CreatorUsername")
                                },
                                Modifier = reader.IsDBNull("ModifierId") ? null : new User
                                {
                                    Id = reader.ToInt("ModifierId"),
                                    Username = reader.ToString("ModifierUsername")
                                },
                            },
                            CreatedDate = reader.ToDateTime("CreatedDateUserFunction"),
                            ModifiedDate = reader.ToDateTime("ModifiedDateUserFunction"),
                            Creator = reader.IsDBNull("CreatorIdUserFunction") ? null : new User
                            {
                                Id = reader.ToInt("CreatorIdUserFunction"),
                                Username = reader.ToString("CreatorUsernameUserFunction")
                            },
                            Modifier = reader.IsDBNull("ModifierIdUserFunction") ? null : new User
                            {
                                Id = reader.ToInt("ModifierIdUserFunction"),
                                Username = reader.ToString("ModifierUsernameUserFunction")
                            }
                        });
                    }

                    return userLevelList;
                }
            }
        }

        /// <summary>
        /// Get user list site function key.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>User list site function key.</returns>
        public Hashtable GetUserListSiteFunctionKeyPolicy(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var hashTable = new Hashtable();

                    while (reader.Read())
                    {
                        var siteId = reader.ToByte("SiteId");
                        if (!hashTable.ContainsKey(siteId))
                        {
                            hashTable.Add(siteId, reader.ToInt("Policy"));
                        }
                    }

                    return hashTable;
                }
            }
        }

        /// <summary>
        /// Get user SSO time.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>User SSO time.</returns>
        public string UserSSOGetTime(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var hashTable = criteria.GetSpParams();

                db.ExecuteNonQuery(hashTable);

                return hashTable["time"].ToString();
            }
        }

        /// <summary>
        /// Read user level.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <returns>User level.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private UserLevel ReadUserLevel(IDataReader reader)
        {
            return new UserLevel
            {
                Level = reader.ToInt("Level"),
                User = reader.IsDBNull("UserId") ? null : new User
                {
                    Id = reader.ToInt("UserId"),
                    FirstName = reader.ToString("FirstName"),
                    LastName = reader.ToString("LastName"),
                    Username = reader.ToString("Username"),
                    Email = reader.ToString("Email"),
                    Password = reader.ToString("UserPwd"),
                    LastLoginDate = reader.ToDateTime("LastLoginDate"),
                    CreatedDate = reader.ToDateTime("CreatedDate"),
                    ModifiedDate = reader.ToDateTime("ModifiedDate"),
                    Group = reader.IsDBNull("GroupId") ? null : new Group
                    {
                        Id = reader.ToShort("GroupId"),
                        Name = reader.ToString("GroupName")
                    },
                    Creator = reader.IsDBNull("CreatorId") ? null : new User
                    {
                        Id = reader.ToInt("CreatorId"),
                        Username = reader.ToString("CreatorUsername")
                    },
                    Modifier = reader.IsDBNull("ModifierId") ? null : new User
                    {
                        Id = reader.ToInt("ModifierId"),
                        Username = reader.ToString("ModifierUsername")
                    },
                },
                CreatedDate = reader.ToDateTime("CreatedDateUserSite"),
                ModifiedDate = reader.ToDateTime("ModifiedDateUserSite"),
                Creator = reader.IsDBNull("CreatorIdUserSite") ? null : new User
                {
                    Id = reader.ToInt("CreatorIdUserSite"),
                    Username = reader.ToString("CreatorUsernameUserSite")
                },
                Modifier = reader.IsDBNull("ModifierIdUserSite") ? null : new User
                {
                    Id = reader.ToInt("ModifierIdUserSite"),
                    Username = reader.ToString("ModifierUsernameUserSite")
                },
                IsActive = reader.ToBool("IsActive"),
                LastLoginDate = reader.ToDateTime("LastLoginDateUserSite"),
                Site = reader.IsDBNull("SiteId") ? null : new Site
                {
                    Id = reader.ToByte("SiteId"),
                    Name = reader.ToString("SiteName")
                }
            };
        }
    }
}