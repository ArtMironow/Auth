using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Auth.Configuration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            //builder.HasData(
            //    new Review
            //    {
            //Id = new Guid("BBD4F7E8-AEF7-4ABF-92D5-6F79243C588E"),
            //        Email = "2@gmail.com",
            //        Title = "Lord of the Rings",
            //        Description = "Very very nice film",
            //        ReviewText = "The title refers to the story's main antagonist, the Dark Lord Sauron, who, in an earlier age, created the One Ring to rule the other Rings of Power given to Men," +
            //                     " Dwarves, and Elves, in his campaign to conquer all of Middle-earth. From homely beginnings in the Shire, a hobbit land reminiscent of the English countryside," +
            //                     " the story ranges across Middle-earth, following the quest to destroy the One Ring, seen mainly through the eyes of the hobbits Frodo, Sam, Merry and Pippin." +
            //                     " Aiding Frodo are the Wizard Gandalf, the Man Aragorn, the Elf Legolas and the Dwarf Gimli, who unite in order to rally the Free Peoples of Middle-earth against" +
            //                     " Sauron's armies and give Frodo a chance to destroy the One Ring in the fire of Mount Doom.",
            //        Theme = "Fantasy",
            //        Image = "",
            //        Rating = 4,
            //        Created = DateTime.Parse(DateTime.Now.ToString()).ToUniversalTime(),
            //        Link = ""
            //    },
            //    new Review
            //    {
            //        Id = new Guid("79589969-D4FB-4609-9268-5FF5E8863CD5"),
            //        Email = "2@gmail.com",
            //        Title = "John wick",
            //        Description = "Very nice action thriller film",
            //ReviewText = "Kolstad's script drew on his interest in action, revenge, and neo noir films. Titled Scorn, producer Basil Iwanyk purchased the rights as his first independent film" +
            //             " production. Reeves, who was experiencing a career lull, liked the script and recommended experienced stunt-and-action choreographers Stahelski and David Leitch to" +
            //             " direct the action scenes but the pair successfully lobbied to co-direct the project. Despite being nearly canceled weeks prior to filming, principal photography on" +
            //             " the project began on October 7, 2013, on a $20–$30 million budget. Stahelski and Leitch focused on highly-choreographed, long, single takes to convey action, eschewing" +
            //             " the rapid cuts and closeup shots of contemporary action films. Iwanyk struggled to secure theatrical distributors because industry executives were dismissive of an" +
            //             " action film by first-time directors, and Reeves's recent films had underperformed. Lionsgate Films eventually purchased the distribution rights and a release date was scheduled for October 24, 2014.",
            //        Theme = "Thriller",
            //        Image = "",
            //        Rating = 4,
            //        Created = DateTime.Parse(DateTime.Now.ToString()).ToUniversalTime(),
            //        Link = ""
            //    }

            //);
        }
    }
}
