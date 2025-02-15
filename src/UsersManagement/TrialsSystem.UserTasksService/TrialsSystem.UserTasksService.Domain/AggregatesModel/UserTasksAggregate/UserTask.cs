﻿using TrialsSystem.UserTasksService.Domain.AggregatesModel.Base;
using TrialsSystem.UserTasksService.Domain.AggregatesModel.Exceptions;

namespace TrialsSystem.UserTasksService.Domain.AggregatesModel.UserTasksAggregate
{
    public class UserTask : Entity
    {
        public string UserId { get; private set; }
        public string Name { get; private set; }
        public UserTaskStatus Status { get; private set; }
        public IDictionary<string, string> AdditionalProperties { get; private set; }

        /// <summary>
        /// Creates UserTask
        /// </summary>
        /// <remarks>UserId and Name is unique pair.</remarks>
        /// <param name="userId">Required</param>
        /// <param name="name">Required</param>
        public UserTask(string userId, string name)
        {
            UserId = !string.IsNullOrWhiteSpace(userId) ? userId : throw new UserTasksDomainException("invalid_patientId");
            SetName(name);
            SetStatus(UserTaskStatus.New);

            SetCreatedDateTimeNow();
            SetLastUpdatedDateTimeNow();

            AdditionalProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddAdditionalProperty(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new UserTasksDomainException("invalid_additionalProperty_key");

            if (AdditionalProperties.ContainsKey(key))
            {
                AdditionalProperties[key] = value;
            }
            else
            {
                AdditionalProperties.Add(key, value);
            }
        }

        public void SetCreatedDateTimeNow()
        {
            CreatedDateTime = DateTime.UtcNow;
        }

        public void SetLastUpdatedDateTimeNow()
        {
            LastUpdatedDateTime = DateTime.UtcNow;
        }

        public void SetStatus(string statusName)
        {
            var status = Enumeration.GetByName<UserTaskStatus>(statusName);

            if (status is null)
                throw new UserTasksDomainException("task_status_not_find");

            SetStatus(status);
        }

        public void SetStatus(UserTaskStatus status)
        {
            if (status == UserTaskStatus.New)
            {
                Status = status;
                return;
            }

            if (status == UserTaskStatus.Reopen)
            {
                if (status == UserTaskStatus.New)
                    throw new UserTasksDomainException("task_status_can_not_be_moved_to_new");

                Status = status;
                return;
            }

            if (status == UserTaskStatus.InProgress)
            {
                if (status == UserTaskStatus.New)
                    throw new UserTasksDomainException("task_status_can_not_be_moved_to_new");

                Status = status;
                return;
            }

            if (status == UserTaskStatus.Completed)
            {
                if (status == UserTaskStatus.New)
                    throw new UserTasksDomainException("task_status_can_not_be_moved_to_new");

                if (status == UserTaskStatus.InProgress)
                    throw new UserTasksDomainException("task_status_can_not_be_moved_to_inprogress");

                Status = status;
            }
        }

        public void ClearAdditionalProperty()
        {
            AdditionalProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void SetName(string name)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new UserTasksDomainException("invalid_task_name");
        }
    }
}
