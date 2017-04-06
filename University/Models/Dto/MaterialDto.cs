using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Models.Tables;

namespace University.Models.Dto
{
    public class MaterialDto
    {
        public MaterialDto()
        {
            Comments = new List<CommentDto>();
        }

        public MaterialDto(Material material)
        {
            Id = material.Id;
            Name = material.Name;
            SubjectId = material.SubjectId;
            FileLink = material.FileLink;
            TypeLesson = material.TypeLesson;
            AuthorId = material.AuthorId;
            DateLoad = material.DateLoad;

            Comments = new List<CommentDto>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int SubjectId { get; set; }

        public string FileLink { get; set; }

        public string TypeLesson { get; set; }

        public string AuthorId { get; set; }

        public DateTime DateLoad { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public List<CommentDto> Comments { get; set; }
    }
}