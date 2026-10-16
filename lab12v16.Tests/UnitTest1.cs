using System;
using System.Collections.Generic;
using System.Linq;
using lab12v16;
using Xunit;

namespace lab12v16.Tests
{
    public class InMemoryRepositoryTests
    {
        [Fact]
        public void Add_ShouldAddItemToRepository()
        {
            var repo = new InMemoryRepository<BlogPost>();
            var post = new BlogPost { Id = 1, Title = "Test Title", Content = "Test Content", AuthorId = 1 };

            repo.Add(post);

            var result = repo.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("Test Title", result.Title);
        }

        [Fact]
        public void GetById_ExistingId_ShouldReturnItem()
        {
            var repo = new InMemoryRepository<BlogPost>();
            var post = new BlogPost { Id = 1, Title = "Post 1", Content = "Content 1", AuthorId = 5 };
            repo.Add(post);

            var result = repo.GetById(1);

            Assert.Equal(1, result.Id);
            Assert.Equal("Post 1", result.Title);
        }

        [Fact]
        public void GetById_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            var repo = new InMemoryRepository<BlogPost>();

            Assert.Throws<KeyNotFoundException>(() => repo.GetById(999));
        }

        [Fact]
        public void GetAll_ShouldReturnAllItems()
        {
            var repo = new InMemoryRepository<BlogPost>();
            repo.Add(new BlogPost { Id = 1, Title = "P1", Content = "C1", AuthorId = 1 });
            repo.Add(new BlogPost { Id = 2, Title = "P2", Content = "C2", AuthorId = 2 });

            var items = repo.GetAll();

            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void Update_ExistingItem_ShouldUpdateCorrectly()
        {
            var repo = new InMemoryRepository<BlogPost>();
            var post = new BlogPost { Id = 1, Title = "Old Title", Content = "Old Content", AuthorId = 1 };
            repo.Add(post);

            var updatedPost = new BlogPost { Id = 1, Title = "New Title", Content = "New Content", AuthorId = 1 };
            repo.Update(updatedPost);

            var result = repo.GetById(1);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Content", result.Content);
        }

        [Fact]
        public void Update_NonExistingItem_ShouldThrowKeyNotFoundException()
        {
            var repo = new InMemoryRepository<BlogPost>();
            var post = new BlogPost { Id = 999, Title = "Missing", Content = "Missing", AuthorId = 1 };

            Assert.Throws<KeyNotFoundException>(() => repo.Update(post));
        }

        [Fact]
        public void Delete_ExistingId_ShouldRemoveItem()
        {
            var repo = new InMemoryRepository<BlogPost>();
            var post = new BlogPost { Id = 1, Title = "To Delete", Content = "Delete Me", AuthorId = 1 };
            repo.Add(post);

            repo.Delete(1);

            Assert.Empty(repo.GetAll());
        }

        [Fact]
        public void Delete_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            var repo = new InMemoryRepository<BlogPost>();

            Assert.Throws<KeyNotFoundException>(() => repo.Delete(999));
        }
    }
}