﻿namespace P7CreateRestApi.Models.DTOs.RatingDTOs
{
    public class RatingUpdateDTO
    {
        public string MoodysRating { get; set; }
        public string SandPRating { get; set; }
        public string FitchRating { get; set; }
        public byte? OrderNumber { get; set; }
    }
}