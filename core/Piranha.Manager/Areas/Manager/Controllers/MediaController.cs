/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Manager;
using Piranha.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class MediaController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public MediaController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the list view for the media.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        [Route("manager/media/{folderId:Guid?}")]
        [Authorize(Policy = Permission.Media)]
        public IActionResult List(Guid? folderId = null) {
            return View("List", Models.MediaListModel.Get(api, folderId));
        }

        /// <summary>
        /// Adds a new media upload.
        /// </summary>
        /// <param name="model">The upload model</param>
        [HttpPost]
        [Route("manager/media/add")]
        [Authorize(Policy = Permission.MediaAdd)]
        public IActionResult Add(Models.MediaUploadModel model) {
            var uploaded = 0;

            foreach (var upload in model.Uploads) {
                if (upload.Length > 0 && !string.IsNullOrWhiteSpace(upload.ContentType)) {
                    using (var stream = upload.OpenReadStream()) {
                        api.Media.Save(new StreamMediaContent() {
                            Id = model.Uploads.Count() == 1 ? model.Id : null,
                            FolderId = model.ParentId,
                            Filename = Path.GetFileName(upload.FileName),
                            Data = stream
                        });
                        uploaded++;
                    }
                }
            }
            if (uploaded == model.Uploads.Count())
                SuccessMessage("Uploaded all media assets.");
            else if (uploaded == 0)
                ErrorMessage("Could not upload the media assets.");
            else InformationMessage($"Uploaded {uploaded} of {model.Uploads.Count()} media assets.");

            return RedirectToAction("List", new { folderId = model.ParentId });
        }

        /// <summary>
        /// Adds a new media upload.
        /// </summary>
        /// <param name="model">The upload model</param>
        [HttpPost]
        [Route("manager/media/modal/add")]
        [Authorize(Policy = Permission.MediaAdd)]
        public IActionResult ModalAdd(Models.MediaUploadModel model) {
            var uploaded = 0;

            foreach (var upload in model.Uploads) {
                if (upload.Length > 0 && !string.IsNullOrWhiteSpace(upload.ContentType)) {
                    using (var stream = upload.OpenReadStream()) {
                        api.Media.Save(new StreamMediaContent() {
                            Id = model.Uploads.Count() == 1 ? model.Id : null,
                            FolderId = model.ParentId,
                            Filename = Path.GetFileName(upload.FileName),
                            Data = stream
                        });
                        uploaded++;
                    }
                }
            }
            return Modal(model.ParentId);
        }

        /// <summary>
        /// Adds a new media folder
        /// </summary>
        /// <param name="model">The model</param>
        [HttpPost]
        [Route("manager/media/addfolder")]
        [Authorize(Policy = Permission.MediaAddFolder)]
        public IActionResult AddFolder(Models.MediaFolderModel model) {
            if (!string.IsNullOrWhiteSpace(model.Name)) {
                api.Media.SaveFolder(new Piranha.Data.MediaFolder() {
                    ParentId = model.ParentId,
                    Name = model.Name
                });
                SuccessMessage($"Added folder \"{model.Name}\".");
            } else {
                ErrorMessage("Name is mandatory when creating a new folder.");
            }
            return RedirectToAction("List", new { folderId = model.ParentId });
        }

        /// <summary>
        /// Deletes the media upload with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("/manager/media/delete/{id:Guid}")]
        [Authorize(Policy = Permission.MediaDelete)]
        public IActionResult Delete(Guid id) {
            var media = api.Media.GetById(id);

            if (media != null) {
                api.Media.Delete(media);
                SuccessMessage($"Deleted \"{media.Filename}\".");
                return RedirectToAction("List", new { folderId = media.FolderId });
            } else {
                ErrorMessage("Could not delete the uploaded media.");
                return RedirectToAction("List", new { folderId = "" });
            }
        }

        /// <summary>
        /// Deletes the folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("/manager/media/delete/folder/{id:Guid}")]
        [Authorize(Policy = Permission.MediaDeleteFolder)]
        public IActionResult DeleteFolder(Guid id) {
            var folder = api.Media.GetFolderById(id);

            if (folder != null) {
                var media = api.Media.GetAll(folder.Id);

                if (media.Count() == 0) {
                    api.Media.DeleteFolder(folder);
                    SuccessMessage($"Deleted folder \"{folder.Name}\".");
                    return RedirectToAction("List", new { folderId = folder.ParentId });
                } else {
                    ErrorMessage($"The folder \"{folder.Name}\" is not empty.");
                    return RedirectToAction("List", new { folderId = folder.ParentId });
                }
            } else {
                ErrorMessage("Could not delete the folder.");
                return RedirectToAction("List", new { folderId = "" });
            }
        }

        [Route("/manager/media/modal/{folderId?}")]
        public IActionResult Modal(Guid? folderId = null) {
            return View("Modal", Models.MediaListModel.Get(api, folderId));            
        }
    }
}