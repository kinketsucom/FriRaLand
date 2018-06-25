using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;        //json

namespace FriRaLand {
    public class FrilSeller {

        public long id;
        public string name;
        public string photo_url;

        public FrilSeller(dynamic json) {
            try {
                this.id = (long)json.id;
                this.name = json.name;
                this.photo_url = json.photourl;
            } catch (Exception e) {

            }
        }


    }
}
