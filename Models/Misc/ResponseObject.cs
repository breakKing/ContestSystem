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

        public static ResponseObject<long> FormResponseObjectForCreation(CreationStatus status, string entityName, long? entityId)
        {
            ResponseObject<long> response = new ResponseObject<long>();
            switch (status)
            {
                case CreationStatus.Success:
                    response = ResponseObject<long>.Success(entityId.GetValueOrDefault(-1));
                    break;
                case CreationStatus.SuccessWithAutoAccept:
                    response = ResponseObject<long>.Success(entityId.GetValueOrDefault(-1));
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
                    response = ResponseObject<long>.Fail(error);
                    break;
                case CreationStatus.ParallelSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.ParallelDbSaveErrorName];
                    response = ResponseObject<long>.Fail(error);
                    break;
                default:
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName]);
                    break;
            }
            return response;
        }

        public static ResponseObject<long> FormResponseObjectForEdition(EditionStatus status, string entityName, long entityId)
        {
            ResponseObject<long> response = new ResponseObject<long>();
            switch (status)
            {
                case EditionStatus.Success:
                    response = ResponseObject<long>.Success(entityId);
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
                    response = ResponseObject<long>.Fail(error);
                    break;
                case EditionStatus.ParallelSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.ParallelDbSaveErrorName];
                    response = ResponseObject<long>.Fail(error);
                    break;
                default:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    response = ResponseObject<long>.Fail(error);
                    break;
            }
            return response;
        }

        public static ResponseObject<long> FormResponseObjectForDeletion(DeletionStatus status, string entityName, long entityId)
        {
            ResponseObject<long> response = new ResponseObject<long>();
            switch (status)
            {
                case DeletionStatus.Success:
                    response = ResponseObject<long>.Success(entityId);
                    break;
                case DeletionStatus.SuccessWithArchiving:
                    response = ResponseObject<long>.Success(entityId);
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
                    response = ResponseObject<long>.Fail(error);
                    break;
                case DeletionStatus.ParallelSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.ParallelDbSaveErrorName];
                    response = ResponseObject<long>.Fail(error);
                    break;
                default:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    response = ResponseObject<long>.Fail(error);
                    break;
            }
            return response;
        }

        public static ResponseObject<long> FormResponseObjectForModeration(ModerationStatus status, string entityName, long entityId)
        {
            ResponseObject<long> response = new ResponseObject<long>();
            switch (status)
            {
                case ModerationStatus.Accepted:
                    response = ResponseObject<long>.Success(entityId);
                    break;
                case ModerationStatus.Rejected:
                    response = ResponseObject<long>.Success(entityId);
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
                    response = ResponseObject<long>.Fail(error);
                    break;
                case ModerationStatus.ParallelSaveError:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.ParallelDbSaveErrorName];
                    response = ResponseObject<long>.Fail(error);
                    break;
                default:
                    error = Constants.ErrorCodes[Constants.CommonSectionName][Constants.UndefinedErrorName];
                    response = ResponseObject<long>.Fail(error);
                    break;
            }
            return response;
        }
    }
}
