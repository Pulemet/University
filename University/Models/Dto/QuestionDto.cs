using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Models.Tables;

namespace University.Models.Dto
{
    public class QuestionDto
    {
        public QuestionDto()
        {
            Answers = new List<AnswerDto>();
        }

        public QuestionDto(Question question)
        {
            Id = question.Id;
            Topic = question.Topic;
            AuthorId = question.AuthorId;
            CreateDate = question.CreateDate;
            SubjectId = question.SubjectId;
            TypeQuestion = question.TypeQuestion;

            Answers = new List<AnswerDto>();
        }
        public int Id { get; set; }

        public string Topic { get; set; }

        public string AuthorId { get; set; }

        public DateTime CreateDate { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public int SubjectId { get; set; }

        public string TypeQuestion { get; set; }

        public List<AnswerDto> Answers { get; set; }
    }
}