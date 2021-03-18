﻿using System.Threading.Tasks;

namespace ContestSystem.Models.Interfaces
{
    public interface IInputModel<BaseModelType, IdType>
    {
        public IdType Id { get; set; }
        public BaseModelType ReadFromInput();
        public Task<BaseModelType> ReadFromInputAsync();
    }
}
