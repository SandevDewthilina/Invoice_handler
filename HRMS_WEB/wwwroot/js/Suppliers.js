import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            _suppliersList: [
                {
                    id: 1,
                    name: 'Kamal Enterprises'
                },
                {
                    id: 2,
                    name: 'Asia Limited'
                },
                {
                    id: 3,
                    name: 'Supun Medicals'
                }
            ]
        }
    },
    computed: {
        suppliers() {
            return this._suppliersList
        }
    },
    methods: {}
});

app.mount("#main");