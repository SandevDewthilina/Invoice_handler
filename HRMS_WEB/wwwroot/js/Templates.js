import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            _templateList: [
                {
                    id: 1,
                    template_name: 'Kamal Enterprises - template 1'
                },
                {
                    id: 2,
                    template_name: 'Asia Limited - template 2'
                },
                {
                    id: 3,
                    template_name: 'Supun Medicals - template 3'
                }
            ]
        }
    },
    computed: {
        templates() {
            return this._templateList
        }
    },
    methods: {}
});

app.mount("#main");