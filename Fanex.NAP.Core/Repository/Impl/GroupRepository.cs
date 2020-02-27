// --------------------------------------------------------------------------
// <copyright file="GroupRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.14">Created</change>
// </history>

namespace Fanex.NAP.Core.Repository.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Command;
    using Fanex.NAP.Core.Criteria.Group;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Interface;

    /// <summary>
    /// Group repository.
    /// </summary>
    public class GroupRepository : BaseRepository<IGroup>, IGroupRepository
    {
        /// <summary>
        /// Get group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Group object.</returns>
        public override IGroup Get(ICriteria criteria)
        {
            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var objGroup = objectDb.QueryEntity<Group, User, User, Group>(
                    (g, cu, mu) => { g.Creator = cu; g.Modifier = mu; return g; },
                    "CreatorId,ModifierId",
                    criteria.GetSpParams());

                return objGroup;
            }
        }

        /// <summary>
        /// Fetch group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of group.</returns>
        public override IEnumerable<IGroup> Fetch(ICriteria criteria)
        {
            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var listGroup = objectDb.Query<Group, User, User, Group>(
                    (g, cu, mu) => { g.Creator = cu; g.Modifier = mu; return g; },
                    "CreatorId,ModifierId",
                    criteria.GetSpParams());

                return listGroup.OrderBy(g => g.Name).ToList<Group>();
            }
        }

        /// <summary>
        /// Insert group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Insert(InsertGroupCriteria criteria, ICriteria logCriteria)
        {
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(hash);
                criteria.GroupId = Convert.ToInt32(objectDb.GetParameterValue("GroupId"));

                return this.GetCommandResult(hash);
            }
        }

        /// <summary>
        /// Update group.
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
        /// Delete group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Delete(ICriteria criteria, ICriteria logCriteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get group policy.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of group policy.</returns>
        public IEnumerable<GroupPolicy> GetGroupPolicy(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var userLevelList = new List<GroupPolicy>();

                    while (reader.Read())
                    {
                        userLevelList.Add(new GroupPolicy
                        {
                            Policy = reader.ToInt("Policy"),
                            Group = reader.IsDBNull("GroupId") ? null : new Group
                            {
                                Id = reader.ToShort("GroupId"),
                                Name = reader.ToString("GroupName"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                                Creator = reader.IsDBNull("CreatorId") ? null : new User
                                {
                                    Id = reader.ToInt("CreatorId"),
                                    Username = reader.ToString("CreatorUsername")
                                },
                                Modifier = reader.IsDBNull("ModifierId") ? null : new User
                                {
                                    Id = reader.ToInt("ModifierId"),
                                    Username = reader.ToString("ModifierUsername")
                                }
                            },
                            CreatedDate = reader.ToDateTime("CreatedDateGroupFunction"),
                            ModifiedDate = reader.ToDateTime("ModifiedDateGroupFunction"),
                            Creator = reader.IsDBNull("CreatorIdGroupFunction") ? null : new User
                            {
                                Id = reader.ToInt("CreatorIdGroupFunction"),
                                Username = reader.ToString("CreatorUsernameGroupFunction")
                            },
                            Modifier = reader.IsDBNull("ModifierIdGroupFunction") ? null : new User
                            {
                                Id = reader.ToInt("ModifierIdGroupFunction"),
                                Username = reader.ToString("ModifierUsernameGroupFunction")
                            }
                        });
                    }

                    return userLevelList.OrderBy(g => g.Group.Name).ToList<GroupPolicy>();
                }
            }
        }
    }
}