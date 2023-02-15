import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            _templateList: []
        }
    },
    computed: {
        templates() {
            return this._templateList
        }
    },
    methods: {},
    created() {
        axios.get('/api/MasterDataApi/GetTemplates').then(resp => {
            this._templateList = resp.data.data
            console.log(this._templateList)
            $(document).ready(function () {
                $("#myuploadstable").DataTable({
                    "responsive": true,
                    "autoWidth": false,
                });
            });
        }).catch(err => {
            console.log(err.message)
        })
    }
});

app.mount("#main");