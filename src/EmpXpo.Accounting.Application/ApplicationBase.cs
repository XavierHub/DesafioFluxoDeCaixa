using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Repositories;

namespace EmpXpo.Accounting.Application
{
    public abstract class ApplicationBase<TEntity> where TEntity : IdentifierBase
    {
        private readonly IRepository<TEntity> _repository;

        public ApplicationBase(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual async Task<TEntity> Create(TEntity model)
        {
            model.CreatedOn = DateTime.Now;
            model.Id = Convert.ToInt32(await _repository.Insert(model));

            return model;
        }

        public virtual async Task<bool> Update(TEntity model)
        {
            var entity = await _repository.GetById(model);

            if (entity.Id == 0)
                return false;

            return await _repository.Update(entity);
        }

        public virtual async Task<bool> Delete(int id)
        {
            var entity = await _repository.GetById(id);
            if (entity.Id == 0)
                return false;

            return await _repository.Delete(entity);
        }

        public virtual async Task<TEntity> Get(int id)
        {
            return await _repository.GetById(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GelAll()
        {
            return await _repository.GetAll();
        }
    }
}
