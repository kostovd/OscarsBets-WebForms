﻿using MovieScrapper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using MovieScrapper.Data.Interfaces;

namespace MovieScrapper.Data
{
    public class CategoryRepository : ICategoryRepository
    {
        public void AddCategory(Category category)
        {
            using (var ctx = new MovieContext())
            {
                ctx.Caterogries.Add(category);
                ctx.SaveChanges();
            }
        }
        

        public void AddNomination(int categoryId, int movieId, List<string> creditIds)
        {
            using (var ctx = new MovieContext())
            {
                var selectedMovie = ctx.Movies
                    .SingleOrDefault(x => x.Id == movieId);

                var selectedCredits = ctx.Credits
                    .Where(x => creditIds.Contains(x.Id))
                    .ToList();

                var selectedCategory = ctx.Caterogries
                    .SingleOrDefault(cat => cat.Id == categoryId);

                Nomination nomination = new Nomination
                {
                    Category = selectedCategory,
                    Movie = selectedMovie,
                    Credits = selectedCredits,
                };

                ctx.Nominations.Add(nomination);
                ctx.SaveChanges();
            }
        }
              
        public void DeleteCategory(int id)
        {

            using (var ctx = new MovieContext())
            {
                var databaseCategory = ctx.Caterogries.Where(x => x.Id == id).SingleOrDefault();
                ctx.Entry(databaseCategory).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }
        }

        public void EditCategory(Category category)
        {
            using (var ctx = new MovieContext())
            {
                ctx.Entry(category).State = System.Data.Entity.EntityState.Modified;
                ctx.SaveChanges();
            }
        }
       
        public IEnumerable<Category> GetAll()
        {
            using (var ctx = new MovieContext())
            {
                var databaseCategory = ctx.Caterogries
                    .Include(cat => cat.Nominations)
                    .Include(cat => cat.Bets.Select(bet => bet.Nomination))
                    .OrderBy(cat => cat.Id)
                    .ToList();

                return databaseCategory;
            }
        }                                          

        public Category GetCategory(int id)
        {
            using (var ctx = new MovieContext())
            {
                var foundedCategory = ctx.Caterogries
                    .Include(cat => cat.Nominations).Where(cat => cat.Id == id).SingleOrDefault();
                return foundedCategory;
            }
        }

        public void MarkAsWinner(int categoryId, int nominationId)
        {
            using (var ctx = new MovieContext())
            {
                var selectedCategory = ctx.Caterogries
                    .Include(cat => cat.Nominations)
                    .Single(x => x.Id == categoryId);

                var winnerNomination = selectedCategory
                    .Nominations
                    .Where(x => x.IsWinner)
                    .SingleOrDefault();

                if (winnerNomination != null)
                {
                    winnerNomination.IsWinner = false;
                }

                var selectedNomination = selectedCategory
                    .Nominations
                    .Single(x => x.Id == nominationId);

                selectedNomination.IsWinner = true;

                ctx.SaveChanges();
            }
        }

        public void RemoveMovieFromCategory(int categoryId, int movieId)
        {
            using (var ctx = new MovieContext())
            {
                var databaseMovie = ctx.Movies.SingleOrDefault(x => x.Id == movieId);
                var databaseCategory = ctx.Caterogries.Include(cat => cat.Nominations).SingleOrDefault(x => x.Id == categoryId);
                var foundedMovie = databaseCategory.Nominations.FirstOrDefault(x => x.Id == movieId);
                databaseCategory.Nominations.Remove(foundedMovie);
                ctx.SaveChanges();
            }
        }                               

    }
}
