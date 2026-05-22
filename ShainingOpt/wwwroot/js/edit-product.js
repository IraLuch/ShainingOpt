  document.querySelector('#fileInput').addEventListener('change',function() {
             const file = this.files[0];
             const preview = document.querySelector('#preview')
             if (file){
                 const reader = new FileReader();
                 reader.onload = function(e){
                     preview.src = e.target.result;
                    preview.style.display = 'block';
                 }
                 reader.readAsDataURL(file);
             }

         })

        document.querySelector('#EditProductForm').addEventListener('submit', async function (e) {
             const fileInput = document.querySelector('#fileInput');
             if (fileInput.files.length > 0){
                   e.preventDefault();
                   const apiKey = '@ViewBag.ImgBbKey';
                   const file = fileInput.files[0];
                   const form = new FormData();
                   form.append('image',file);

                   const response = await fetch(`https://api.imgbb.com/1/upload?key=${apiKey}`, {
                       method:'POST',
                       body: form
                   });

                   const res = await response.json();
                   if (res.success){
                       document.getElementById('hiddenImageUrl').value = res.data.url;
                       this.submit();
                   }

             }



         })
             $.validator.methods.range = function (value, element, param) {
             var globalizedValue = value.replace(",", ".");
             return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
         }

         $.validator.methods.number = function (value, element) {
             return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);}