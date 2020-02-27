// --------------------------------------------------------------------------
// <copyright file="MenuItemRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.13">Created</change>
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
    using Fanex.NAP.Core.Criteria.MenuItem;
    using Fanex.NAP.Core.Criteria.Misc;
    using Fanex.NAP.Core.Domain;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Repository.Interface;

    /// <summary>
    /// Menu item repository.
    /// </summary>
    public class MenuItemRepository : BaseRepository<IMenuItem>, IMenuItemRepository
    {
        /// <summary>
        /// Get menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Menu item object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public override IMenuItem Get(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader.Read())
                    {
                        return new MenuItem
                        {
                            Id = reader.ToInt("MenuItemId"),
                            Name = reader.ToString("MenuItemName"),
                            Function = new Function
                            {
                                Id = reader.ToShort("FunctionId"),
                                Name = reader.ToString("FunctionName")
                            },
                            Site = new Site
                            {
                                Id = reader.ToByte("SiteId"),
                                Name = reader.ToString("SiteName")
                            },
                            Description = reader.ToString("Description"),
                            Parent = reader.IsDBNull("ParentId") ? null : new MenuItem
                            {
                                Id = reader.ToInt("ParentId"),
                                Name = reader.ToString("ParentName")
                            },
                            Policy = reader.ToInt("Policy"),
                            Url = reader.ToString("URL"),
                            DisplayOrder = reader.ToShort("DisplayOrder"),
                            IsActive = reader.ToBool("IsActive"),
                            Creator = reader.IsDBNull("CreatorId") ? null : new User
                            {
                                Id = reader.ToInt("CreatorId"),
                                Name = reader.ToString("CreatorUsername")
                            },
                            CreatedDate = reader.ToDateTime("CreatedDate"),
                            Modifier = reader.IsDBNull("ModifierId") ? null : new User
                            {
                                Id = reader.ToInt("ModifierId"),
                                Name = reader.ToString("ModifierUsername")
                            },
                            ModifiedDate = reader.ToDateTime("ModifiedDate"),
                            Environment = reader.ToString("Environment")
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Count menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Number of menu item.</returns>
        public override int Count(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return (int)db.ExecuteScalar(criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Fetch menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of menu item.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public override IEnumerable<IMenuItem> Fetch(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    var menuItemList = new List<MenuItem>();

                    while (reader.Read())
                    {
                        menuItemList.Add(new MenuItem
                        {
                            Id = reader.ToInt("MenuItemId"),
                            Name = reader.ToString("MenuItemName"),
                            Function = new Function
                            {
                                Id = reader.ToShort("FunctionId"),
                                Name = reader.ToString("FunctionName")
                            },
                            Site = new Site
                            {
                                Id = reader.ToByte("SiteId"),
                                Name = reader.ToString("SiteName")
                            },
                            Description = reader.ToString("Description"),
                            Parent = reader.IsDBNull("ParentId") ? null : new MenuItem
                            {
                                Id = reader.ToInt("ParentId"),
                                Name = reader.ToString("ParentName")
                            },
                            Policy = reader.ToInt("Policy"),
                            Url = reader.ToString("URL"),
                            DisplayOrder = reader.ToShort("DisplayOrder"),
                            IsActive = reader.ToBool("IsActive"),
                            Creator = reader.IsDBNull("CreatorId") ? null : new User
                            {
                                Id = reader.ToInt("CreatorId"),
                                Name = reader.ToString("CreatorUsername")
                            },
                            CreatedDate = reader.ToDateTime("CreatedDate"),
                            Modifier = reader.IsDBNull("ModifierId") ? null : new User
                            {
                                Id = reader.ToInt("ModifierId"),
                                Name = reader.ToString("ModifierUsername")
                            },
                            ModifiedDate = reader.ToDateTime("ModifiedDate"),
                            Environment = reader.ToString("Environment")
                        });
                    }

                    return menuItemList.OrderBy(m => m.Name).ToList<MenuItem>();
                }
            }
        }

        /// <summary>
        /// Insert menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Insert(InsertMenuItemCriteria criteria, ICriteria logCriteria)
        {
            // prepare SP parameters
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(hash);

                criteria.MenuItemId = Convert.ToInt32(db.GetParameterValue("MenuItemId"));

                return this.GetCommandResult(hash);
            }
        }

        /// <summary>
        /// Update menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Update(ICriteria criteria, ICriteria logCriteria)
        {
            // prepare SP parameters
            Hashtable hash = criteria.GetSpParams();

            hash = logCriteria.AddSpParams(hash);
            hash = (new ResultCriteria()).AddSpParams(hash);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(hash);

                return this.GetCommandResult(hash);
            }
        }

        /// <summary>
        /// Delete menu item.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="logCriteria">The log criteria.</param>
        /// <returns>Command result.</returns>
        public ICommandResult Delete(ICriteria criteria, ICriteria logCriteria)
        {
            throw new NotImplementedException();
        }
    }
}