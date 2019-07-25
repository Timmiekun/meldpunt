tinymce.init({
  selector: ".richtext",
  relative_urls: false,
  remove_script_host: true,
  height: 400,
  image_class_list: [
    { title: 'none', value: '' },
    { title: 'left', value: 'left' },
    { title: 'right', value: 'right' }
  ],
  image_title: true,
  plugins: [
    "advlist autolink lists link image charmap print preview anchor",
    "searchreplace visualblocks code fullscreen",
    "insertdatetime media table contextmenu paste"
  ],
  file_picker_callback: function (callback, value, meta) {
    if (meta.filetype == 'image') {
      OpenModal('image', callback, true);
    }
  },
  toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
});