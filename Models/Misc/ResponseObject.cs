using ContestSystem.Models.Dictionaries;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Models.Misc
{
    public class ResponseObject<TData>
    {
        public bool Status { get; set; }
        public TData Data { get; set; }
        public List<string> Errors { get; set; }

        public static ResponseObject<TData> Success(TData data)
        {
            return new ResponseObject<TData>
            {
                Status = true,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static ResponseObject<TData> Fail(params string[] errorsList)
        {
            return new ResponseObject<TData>
            {
                Status = false,
                Errors = new List<string>(errorsList),
                Data = default
            };
        }

        public static ResponseObject<TData> Fail(ModelStateDictionary modelState, string entityName)
        {
            string error = string.Empty;
            if (Constants.ErrorCodes.TryGetValue(entityName, out Dictionary<string, string> codes))
            {
                error = codes.GetValueOrDefault(Constants.ValidationFailedErrorName,
                                                Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
            }
            else
            {
                error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
            }
            return Fail(modelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage)
                                    .Prepend(error)
                                    .ToArray());
        }

        public static ResponseObject<TData> FormResponseObjectForCreation(CreationStatus status, string entityName, TData entityId)
        {
            ResponseObject<TData> response = new ResponseObject<TData>();
            switch (status)
            {
                case CreationStatus.Success:
                    response = Success(entityId);
                    break;
                case CreationStatus.SuccessWithAutoAccept:
                    response = Success(entityId);
                    break;
                case CreationStatus.LimitExceeded:
                    string error = string.Empty;
                    if (Constants.ErrorCodes.TryGetValue(entityName, out Dictionary<string, string> codes))
                    {
                        error = codes.GetValueOrDefault(Constants.CreationLimitExceededErrorName,
                                                        Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    }
                    response = Fail(error);
                    break;
                case CreationStatus.DbSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName];
                    response = Fail(error);
                    break;
                default:
                    response = Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    break;
            }
            return response;
        }

        public static ResponseObject<TData> FormResponseObjectForEdition(EditionStatus status, string entityName, TData entityId)
        {
            ResponseObject<TData> response = new ResponseObject<TData>();
            switch (status)
            {
                case EditionStatus.Success:
                    response = Success(entityId);
                    break;
                case EditionStatus.NotExistentEntity:
                    string error = string.Empty;
                    if (Constants.ErrorCodes.TryGetValue(entityName, out Dictionary<string, string> codes))
                    {
                        error = codes.GetValueOrDefault(Constants.EntityDoesntExistErrorName,
                                                        Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    }
                    response = Fail(error);
                    break;
                case EditionStatus.DbSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName];
                    response = Fail(error);
                    break;
                case EditionStatus.ContestLocked:
                    error = Constants.ErrorCodes[Constants.ContestEntityName][Constants.LockedErrorName];
                    response = Fail(error);
                    break;
                case EditionStatus.ArchivedAndRecreated:
                    response = Success(entityId);
                    break;
                default:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    response = Fail(error);
                    break;
            }
            return response;
        }

        public static ResponseObject<TData> FormResponseObjectForDeletion(DeletionStatus status, string entityName, TData entityId)
        {
            ResponseObject<TData> response = new ResponseObject<TData>();
            switch (status)
            {
                case DeletionStatus.Success:
                    response = Success(entityId);
                    break;
                case DeletionStatus.SuccessWithArchiving:
                    response = Success(entityId);
                    break;
                case DeletionStatus.NotExistentEntity:
                    string error = string.Empty;
                    if (Constants.ErrorCodes.TryGetValue(entityName, out Dictionary<string, string> codes))
                    {
                        error = codes.GetValueOrDefault(Constants.EntityDoesntExistErrorName,
                                                        Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    }
                    response = Fail(error);
                    break;
                case DeletionStatus.DbSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName];
                    response = Fail(error);
                    break;
                case DeletionStatus.Blocked:
                    error = string.Empty;
                    if (Constants.ErrorCodes.TryGetValue(entityName, out codes))
                    {
                        error = codes.GetValueOrDefault(Constants.DeleteionBlockedErrorName,
                                                        Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    }
                    response = Fail(error);
                    break;
                default:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    response = Fail(error);
                    break;
            }
            return response;
        }

        public static ResponseObject<TData> FormResponseObjectForModeration(ModerationStatus status, string entityName, TData entityId)
        {
            ResponseObject<TData> response = new ResponseObject<TData>();
            switch (status)
            {
                case ModerationStatus.Accepted:
                    response = Success(entityId);
                    break;
                case ModerationStatus.Rejected:
                    response = Success(entityId);
                    break;
                case ModerationStatus.NotExistentEntity:
                    string error = string.Empty;
                    if (Constants.ErrorCodes.TryGetValue(entityName, out Dictionary<string, string> codes))
                    {
                        error = codes.GetValueOrDefault(Constants.EntityDoesntExistErrorName,
                                                        Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    }
                    response = Fail(error);
                    break;
                case ModerationStatus.DbSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName];
                    response = Fail(error);
                    break;
                default:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    response = Fail(error);
                    break;
            }
            return response;
        }

        public static ResponseObject<TData> FormResponseObjectForFormCheck(FormCheckStatus status, string entityName, TData entityId = default)
        {
            ResponseObject<TData> response = new ResponseObject<TData>();
            switch (status)
            {
                case FormCheckStatus.Correct:
                    response = Success(entityId);
                    break;
                case FormCheckStatus.NonExistentCompiler:
                    response = Fail(Constants.ErrorCodes[Constants.CompilerEntityName][Constants.EntityDoesntExistErrorName]);
                    break;
                case FormCheckStatus.NonExistentParticipant:
                    response = Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserNotInContestErrorName]);
                    break;
                case FormCheckStatus.NonExistentContest:
                    response = Fail(Constants.ErrorCodes[Constants.ContestEntityName][Constants.EntityDoesntExistErrorName]);
                    break;
                case FormCheckStatus.NonExistentProblem:
                    response = Fail(Constants.ErrorCodes[Constants.ProblemEntityName][Constants.EntityDoesntExistErrorName]);
                    break;
                case FormCheckStatus.NonExistentRulesSet:
                    response = Fail(Constants.ErrorCodes[Constants.RulesSetEntityName][Constants.EntityDoesntExistErrorName]);
                    break;
                case FormCheckStatus.NonExistentChecker:
                    response = Fail(Constants.ErrorCodes[Constants.CheckerEntityName][Constants.EntityDoesntExistErrorName]);
                    break;
                case FormCheckStatus.NonExistentUser:
                    response = Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
                    break;
                case FormCheckStatus.ExistentSolution:
                    response = Fail(Constants.ErrorCodes[Constants.SolutionEntityName][Constants.EntityAlreadyExistsErrorName]);
                    break;
                case FormCheckStatus.LimitExceeded:
                    string error = string.Empty;
                    if (Constants.ErrorCodes.TryGetValue(entityName, out Dictionary<string, string> codes))
                    {
                        error = codes.GetValueOrDefault(Constants.CreationLimitExceededErrorName,
                                                        Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    }
                    response = Fail(error);
                    break;
                case FormCheckStatus.NonExistentChatUser:
                    response = Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserNotInChatErrorName]);
                    break;
                case FormCheckStatus.NonExistentSolution:
                    response = Fail(Constants.ErrorCodes[Constants.SolutionEntityName][Constants.UserNotInChatErrorName]);
                    break;
                case FormCheckStatus.WrongMoment:
                    error = string.Empty;
                    if (Constants.ErrorCodes.TryGetValue(entityName, out codes))
                    {
                        error = codes.GetValueOrDefault(Constants.WrongMomentForEditingErrorName,
                                                        Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    }
                    else
                    {
                        error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    }
                    response = Fail(error);
                    break;
                default:
                    response = Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    break;
            }
            return response;
        }

        public static ResponseObject<bool> FormResponseObjectForInvitation(InviteStatus status)
        {
            var response = new ResponseObject<bool>();

            switch (status)
            {
                case InviteStatus.Pending:
                    response = ResponseObject<bool>.Success(false);
                    break;
                case InviteStatus.Added:
                    response = ResponseObject<bool>.Success(true);
                    break;
                case InviteStatus.UserAlreadyInvited:
                    response = ResponseObject<bool>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserAlreadyInvitedErrorName]);
                    break;
                case InviteStatus.UserAlreadyInEntity:
                    response = ResponseObject<bool>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityAlreadyExistsErrorName]);
                    break;
                case InviteStatus.DbSaveError:
                    response = ResponseObject<bool>.Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName]);
                    break;
                default:
                    response = ResponseObject<bool>.Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    break;
            }

            return response;
        }
    }
}
