﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nauktion.Models;

namespace Nauktion.Repositories
{
    public class AuktionRepository : IAuktionRepository
    {
        [NotNull]
        public Uri BaseAddress { get; } = new Uri(@"http://nackowskis.azurewebsites.net/api/");

        public int Gruppkod { get; } = 1;

        private readonly HttpClient _client;

        public AuktionRepository()
        {
            _client = new HttpClient {BaseAddress = BaseAddress};
        }

        ~AuktionRepository()
        {
            _client?.Dispose();
        }

        public async Task<List<BudModel>> ListBudsAsync(int auktionID)
        {
            HttpResponseMessage response = await _client.GetAsync($"bud/{Gruppkod}/{auktionID}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<List<BudModel>>()
                ?? new List<BudModel>();
        }

        public async Task CreateBudAsync(BudModel model)
        {
            var formdata = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                [nameof(model.AuktionID)] = model.AuktionID.ToString(),
                [nameof(model.Budgivare)] = model.Budgivare,
                [nameof(model.Summa)] = model.Summa.ToString()
            });

            HttpResponseMessage response = await _client.PostAsync("bud", formdata);

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<AuktionModel>> ListAuktionsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync($"Auktion/{Gruppkod}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<List<AuktionModel>>()
                ?? new List<AuktionModel>();
        }

        public async Task<AuktionModel> GetAuktionAsync(int id)
        {
            HttpResponseMessage response = await _client.GetAsync($"Auktion/{Gruppkod}/{id}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<AuktionModel>();
        }
    }
}