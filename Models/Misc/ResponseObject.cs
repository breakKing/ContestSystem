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

        public static ResponseObject<TIdentity> Fail(ModelStateDictionary modelState, string errorCode)
        {
            return Fail(modelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage)
                                    .Prepend(errorCode)
                                    .ToArray());
        }
    }
}
