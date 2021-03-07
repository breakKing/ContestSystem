using ContestSystem.Models.Interfaces;
using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models.Output
{
    public class ContestInProgressOutputModel : IOutputModel<ContestBaseModel>
    {
        public string Name { get; set; }
        public List<ProblemOutputModel> Problems { get; set; } = new List<ProblemOutputModel>();
        public List<MonitorEntryOutputModel> MonitorEntries { get; set; } = new List<MonitorEntryOutputModel>();
        public List<MessageOutputModel> PublicMessages { get; set; } = new List<MessageOutputModel>();

        public void TransformForOutput(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Name = baseModel.Name;

            Problems = (List<ProblemOutputModel>)dbContext.ContestsProblems.Include(cp => cp.Problem)
                                                                              .Where(cp => cp.ContestId == baseModel.Id)
                                                                              .Select(cp => cp.Problem)
                                                                              .ToList()
                                                                              .ConvertAll(p =>
                                                                              {
                                                                                  ProblemOutputModel pOut = new ProblemOutputModel();
                                                                                  pOut.TransformForOutput(p, dbContext);
                                                                                  return pOut;
                                                                              })
                                                                              .OrderBy(pOut => pOut.Alias);

            MonitorEntries = (List<MonitorEntryOutputModel>)dbContext.ContestsParticipants.Include(cp => cp.Participant)
                                                                                            .Where(cp => cp.ContestId == baseModel.Id)
                                                                                            .Select(cp => cp.Participant)
                                                                                            .ToList()
                                                                                            .ConvertAll(p =>
                                                                                            {
                                                                                                MonitorEntryOutputModel mOut = new MonitorEntryOutputModel();
                                                                                                mOut.TransformForOutput(p, dbContext);
                                                                                                return mOut;
                                                                                            })
                                                                                            .OrderBy(mOut => mOut.Position);

            PublicMessages = dbContext.Messages.Where(m => m.ContestId == baseModel.Id && m.IsPublic)
                                                .ToList()
                                                .ConvertAll(m =>
                                                {
                                                    MessageOutputModel mOut = new MessageOutputModel();
                                                    mOut.TransformForOutput(m, dbContext);
                                                    return mOut;
                                                });
        }

        public async Task TransformForOutputAsync(ContestBaseModel baseModel, ContestSystemDbContext dbContext)
        {
            Name = baseModel.Name;

            List<ProblemBaseModel> problemsQuery = await dbContext.ContestsProblems.Include(cp => cp.Problem)
                                                                                    .Where(cp => cp.ContestId == baseModel.Id)
                                                                                    .Select(cp => cp.Problem)
                                                                                    .ToListAsync();
            Problems = (List<ProblemOutputModel>)problemsQuery.ConvertAll(async p =>
                                                                {
                                                                    ProblemOutputModel pOut = new ProblemOutputModel();
                                                                    await pOut.TransformForOutputAsync(p, dbContext);
                                                                    return pOut;
                                                                })
                                                                .OrderBy(pOut => pOut.Result.Alias);

            List<UserBaseModel> participantsContestQuery = await dbContext.ContestsParticipants.Include(cp => cp.Participant)
                                                                                                .Where(cp => cp.ContestId == baseModel.Id)
                                                                                                .Select(cp => cp.Participant)
                                                                                                .ToListAsync();
            MonitorEntries = (List<MonitorEntryOutputModel>)participantsContestQuery.ConvertAll(async p =>
                                                                                        {
                                                                                            MonitorEntryOutputModel mOut = new MonitorEntryOutputModel();
                                                                                            await mOut.TransformForOutputAsync(p, dbContext);
                                                                                            return mOut;
                                                                                        })
                                                                                        .Select(mOut => mOut.Result);

            List<MessageBaseModel> messagesQuery = await dbContext.Messages.Where(m => m.ContestId == baseModel.Id && m.IsPublic)
                                                                            .ToListAsync();
            PublicMessages = (List<MessageOutputModel>)messagesQuery.ConvertAll(async m =>
                                                                    {
                                                                        MessageOutputModel mOut = new MessageOutputModel();
                                                                        await mOut.TransformForOutputAsync(m, dbContext);
                                                                        return mOut;
                                                                    })
                                                                    .Select(mOut => mOut.Result);
        }
    }
}
