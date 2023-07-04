using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using TV_DASH_API.Models;


namespace TV_DASH_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CebuImageController : ControllerBase
    {
        private readonly ImageDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CebuImageController(ImageDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        /* [HttpGet]
         public ActionResult<IEnumerable<ImageModel>> GetImages(int floor)
         {
             string folderName = $"Floor_{floor}_Images";
             string folderPath = Path.Combine(_hostEnvironment.ContentRootPath, folderName);

             if (!Directory.Exists(folderPath))
             {
                 return NotFound();
             }

             var imagePaths = Directory.GetFiles(folderPath);
             var imageModels = imagePaths.Select(imagePath => new ImageModel()
             {
                 ImageName = Path.GetFileName(imagePath),
                 ImageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{folderName}/{Path.GetFileName(imagePath)}"
             });

             return Ok(imageModels);
         }
        */

        [HttpGet]
        [Route("GetAllCebuImages")]
        public async Task<ActionResult<IEnumerable<CebuImageModel>>> GetAllImages()
        {
            int[] cebuFloors = Enumerable.Range(1, 11).ToArray();
            string root = "Images";
            var imageModels = new List<CebuImageModel>();

            foreach (int cebuFloor in cebuFloors)
            {
                string folderName = $"CebuFloor_{cebuFloor}_Images";
                var images = await _context.TVDash_CebuImages
                    .Where(x => x.Floor == cebuFloor)
                    .Select(x => new CebuImageModel()
                    {
                        Content= x.Content,
                        ImageKey=x.ImageKey,
                        ImageID = x.ImageID,
                        Order = x.Order,
                        Floor = x.Floor,
                        ImageName = x.ImageName,
                        ImageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{root}/{folderName}/{x.ImageName}"
                    })
                    .OrderBy(x => x.Order)
                    .ToListAsync();
                   
                imageModels.AddRange(images);
            }

            return imageModels;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CebuImageModel>>> GetImages(int cebufloor)
        {
            string root = "Images";
            string folderName = $"CebuFloor_{cebufloor}_Images";
            return await _context.TVDash_CebuImages
                .Where(x => x.Floor == cebufloor)
                .Select(x => new CebuImageModel()
                {
                    ImageID = x.ImageID,
                    Floor = x.Floor,
                    Order = x.Order,
                    ImageName = x.ImageName,
                    //  ImageSrc = String.Format("{0}://{1}{2}/$"Floor_{ floor }_Images"/{3}", Request.Scheme, Request.Host, Request.PathBase, x.ImageName)
                    ImageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{root}/{folderName}/{x.ImageName}"
                })
               .OrderBy(x => x.Order)
                .ToListAsync();

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CebuImageModel>> GetImageModel(int id)
        {
            var imageModel = await _context.TVDash_CebuImages.FindAsync(id);

            if (imageModel == null)
            {
                return NotFound();
            }

            return imageModel;
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImageModel(int id, [FromForm] CebuImageModel imageModel, int cebufloor)
        {
            if (id != imageModel.ImageID)
            {
                return BadRequest();
            }

            if (imageModel.ImageFile != null)
            {
                DeleteImage(imageModel.ImageName, id);
                imageModel.ImageName = await SaveImage(imageModel.ImageFile, cebufloor);
            }

            _context.Entry(imageModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPost]
        [Route("api/images/reorder")]
        public async Task<IActionResult> ReorderImagesAsync([FromBody] List<int> imageIds)
        {
            // Fetch the images from the database based on the imageIds
            var images = _context.TVDash_CebuImages.Where(image => imageIds.Contains(image.ImageID)).ToList();



            // Update the order of the images based on the received imageIds
            foreach (var image in images)
            {
                var newIndex = imageIds.IndexOf(image.ImageID);
                image.Order = newIndex;


            }

            await _context.SaveChangesAsync();


            return Ok();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> UploadImages(List<IFormFile> files, int cebufloor, string content)
        {
            foreach (var file in files)
            {
                Guid key = Guid.NewGuid();
                var imageName = await SaveImage(file, cebufloor);
                var imageModel = new CebuImageModel { ImageName = imageName, Floor = cebufloor, ImageKey = key, Content = content };

                _context.TVDash_CebuImages.Add(imageModel);

            }

            await _context.SaveChangesAsync();

            return Ok("Success");
        }

       /* [HttpPost]
        [Route("api/AddRemarks")]
        public async Task<IActionResult> AddRemarks(int cebufloor, string content)
        {
            var remarks = new CebuImageModel { Floor = cebufloor, Content = content };
            _context.TVDash_CebuImages.Add(remarks);

        
        await _context.SaveChangesAsync();
            return Ok("Success");
    }*/
        [HttpDelete("{id}")]
        public async Task<ActionResult<CebuImageModel>> DeleteImageModel(int id)
        {
            var imageModel = await _context.TVDash_CebuImages.FindAsync(id);
            if (imageModel == null)
            {
                return NotFound();
            }
            DeleteImage(imageModel.ImageName, id);
            _context.TVDash_CebuImages.Remove(imageModel);
            await _context.SaveChangesAsync();
            //return imageModel;
            return this.Content("application/json");
        }




        private bool ImageModelExists(int id)
        {
            return _context.TVDash_CebuImages.Any(e => e.ImageID == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile, int floor)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(20).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("-yy-MM-dd-hhmms") + Path.GetExtension(imageFile.FileName);
            string root = "Images";
            string folderName = $"CebuFloor_{floor}_Images";
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, root, folderName, imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }


        [NonAction]
        public void DeleteImage(string imageName, int floor)
        {
            string root = "Images";
            string folderName = $"CebuFloor_{floor}_Images";
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, root, folderName, imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }


    }
}
