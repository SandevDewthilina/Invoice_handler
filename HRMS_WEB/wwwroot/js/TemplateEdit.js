import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            form: {
                template_name: '',
                templateRegexList: []
            }
        }
    },
    computed: {
        getForm() {
            return this.form
        },
        getName() {
            return this.form.template_name
        },
        getRegexComponents() {
            return this.form.templateRegexList
        }
    },
    methods: {
        addRow() {
            let regexList = this.form.templateRegexList
            let lastId = -1;
            if(regexList.length > 0) {
                lastId = regexList[regexList.length - 1].id
            }
            const newId = lastId + 1;
            regexList.push({
                id: newId,
                key: '',
                value: '',
                area: '',
                isArea: false
            })
            this.form.templateRegexList = regexList
        },
        deleteRow(id) {
            console.log(id)
            const obj = this.form.templateRegexList.findIndex(e => e.id === id)
            this.form.templateRegexList.splice(obj, 1)
        },
        submit(e) {
            e.preventDefault()
            const params = new Proxy(new URLSearchParams(window.location.search), {
                get: (searchParams, prop) => searchParams.get(prop),
            });
            axios.post('/api/MasterDataApi/EditTemplate?Id=' + params.id, this.form).then(resp => {
                if(resp.data.success) {
                    window.location.href = '/Home/ViewTemplates'
                } else {
                    alert('request was not successful')
                }
            }).catch(err => {
                console.log(err.message)
                alert(err.message)
            })
        }
    },
    created() {
        const params = new Proxy(new URLSearchParams(window.location.search), {
            get: (searchParams, prop) => searchParams.get(prop),
        });
        
        axios.get('/api/MasterDataApi/GetTemplateForId?Id=' + params.id).then(resp => {
            console.log(resp.data)
            this.form = resp.data.data.form
        }).catch(err => {
            
        })
    }
});

app.mount("#main");