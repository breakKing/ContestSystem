using ContestSystem.Models.Dictionaries;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Models.Misc
{
    public class ResponseObject<TIdentity>
    {
        public bool Status { get; set; }
        public TIdentity Data { get; set; }
        public List<string> Errors { get; set; }

        public static ResponseObject<TIdentity> Success(TIdentity id)
        {
            return new ResponseObject<TIdentity>
            {
                Status = true,
                Data = id,
                Errors = new List<string>()
            };
        }

        public static ResponseObject<TIdentity> Fail(params string[] errorsList)
        {
            return new ResponseObject<TIdentity>
            {
                Status = false,
                Errors = new List<string>(errorsList),
                Data = default
            };
        }

        public static ResponseObject<TIdentity> Fail(ModelStateDictionary modelState, string entityName)
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

        public static ResponseObject<TIdentity> FormResponseObjectForCreation(CreationStatus status, string entityName, TIdentity entityId)
        {
            ResponseObject<TIdentity> response = new ResponseObject<TIdentity>();
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

        public static ResponseObject<TIdentity> FormResponseObjectForEdition(EditionStatus status, string entityName, TIdentity entityId)
        {
            ResponseObject<TIdentity> response = new ResponseObject<TIdentity>();
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

        public static ResponseObject<TIdentity> FormResponseObjectForDeletion(DeletionStatus status, string entityName, TIdentity entityId)
        {
            ResponseObject<TIdentity> response = new ResponseObject<TIdentity>();
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
                default:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    response = Fail(error);
                    break;
            }
            return response;
        }

        public static ResponseObject<TIdentity> FormResponseObjectForModeration(ModerationStatus status, string entityName, TIdentity entityId)
        {
            ResponseObject<TIdentity> response = new ResponseObject<TIdentity>();
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

        public static ResponseObject<TIdentity> FormResponseObjectForFormCheck(FormCheckStatus status, string entityName, TIdentity entityId = default)
        {
            ResponseObject<TIdentity> response = new ResponseObject<TIdentity>();
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
                default:
                    response = Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    break;
            }
            return response;
        }
    }
}
