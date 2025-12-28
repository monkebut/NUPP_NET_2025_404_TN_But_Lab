using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaManagement.Common;
using CinemaManagement.Infrastructure.Mappers;

namespace CinemaManagement.Infrastructure
{
    /// <summary>
    /// Wrapper for EfCrudServiceAsync that works with Common models but uses Infrastructure models internally
    /// </summary>
    public class EfCrudServiceWrapper<TCommon, TModel> : ICrudServiceAsync<TCommon>
        where TCommon : class, IHasId
        where TModel : class, IHasId
    {
        private readonly EfCrudServiceAsync<TModel> _efService;
        private readonly Func<TCommon, TModel> _toModel;
        private readonly Func<TModel, TCommon> _fromModel;

        public EfCrudServiceWrapper(
            EfCrudServiceAsync<TModel> efService,
            Func<TCommon, TModel> toModel,
            Func<TModel, TCommon> fromModel)
        {
            _efService = efService ?? throw new ArgumentNullException(nameof(efService));
            _toModel = toModel ?? throw new ArgumentNullException(nameof(toModel));
            _fromModel = fromModel ?? throw new ArgumentNullException(nameof(fromModel));
        }

        public async Task<bool> CreateAsync(TCommon element)
        {
            var model = _toModel(element);
            return await _efService.CreateAsync(model);
        }

        public async Task<TCommon> ReadAsync(Guid id)
        {
            var model = await _efService.ReadAsync(id);
            if (model == null)
                return null!;
            return _fromModel(model);
        }

        public async Task<IEnumerable<TCommon>> ReadAllAsync()
        {
            var models = await _efService.ReadAllAsync();
            return models.Select(_fromModel).ToList();
        }

        public async Task<IEnumerable<TCommon>> ReadAllAsync(int page, int amount)
        {
            var models = await _efService.ReadAllAsync(page, amount);
            return models.Select(_fromModel).ToList();
        }

        public async Task<bool> UpdateAsync(TCommon element)
        {
            var model = _toModel(element);
            return await _efService.UpdateAsync(model);
        }

        public async Task<bool> RemoveAsync(TCommon element)
        {
            var model = _toModel(element);
            return await _efService.RemoveAsync(model);
        }

        public async Task<bool> SaveAsync()
        {
            return await _efService.SaveAsync();
        }

        public IEnumerator<TCommon> GetEnumerator()
        {
            var models = _efService.ReadAllAsync().Result;
            return models.Select(_fromModel).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

