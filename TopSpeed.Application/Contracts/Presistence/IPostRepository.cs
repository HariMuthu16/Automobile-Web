﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSpeed.Domain.Models;

namespace TopSpeed.Application.Contracts.Presistence
{
    public interface IPostRepository : IGenericRepository<Post>
    {
         Task Update(Post post);
         Task<Post> GetPostById(Guid id);
         Task<List<Post>> GetAllPost();
    }
}
