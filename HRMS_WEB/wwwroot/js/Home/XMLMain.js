var tags = taglist; // passed from the other script

var mainfirst = true // identify the first push to dom event

$(document).ready(function () {

    var maintags = $('.maintags')
    var attributediv = $('.attributecontainer')
    var injectparent = $(".injectparent")
    var domainpush = $('.pushtoparent')
    var dom = $('.xmldom')
    var clear = $('.clearbtn')

    var parentnode = null
    var treedata = []

    // select event of the main tag list drop down
    maintags.change(function () {

        if (!mainfirst) {
            attributediv.empty()
        }

        // jQuery
        var maintagid = $(this).find(':selected').val();
        var maintagvalue = $(this).find(':selected').text();

        var attributes = tags.filter(item => item.id == maintagid)[0].attributes

        attributediv.append(` <div class="form-group">
                                <label class="control-label">Fill Attributes</label>
                                <div class="col attributelist"></div>
                              </div>`)

        var attributeselect = $('.attributelist')

        // create attribute form for data insert
        attributes.forEach(att => {
            attributeselect.append(`<div class="input-group mb-3">
                                        <div class="input-group-prepend">
                                            <button type="button" class="btn btn-outline-primary text-left" style="width: 210px">` + att.name + " (" + att.datatype + ")" + `</button>
                                        </div>
                                        <!-- /btn-group -->
                                        <input class="form-control" id="` + att.id + `" value="` + att.defaultValue + `" />
                                    </div>`)
        })

        // create parent dropdown for the firstime
        if (mainfirst) {
            injectparent.append(`<div class="form-group">
                                <label class="control-label">Parent Node</label>
                                <select class="form-control select2 parentlist"></select>
                              </div>`)
            var parentlist = $('.parentlist')
            // id-level-position
            parentlist.append(`<option></option>`)
            parentlist.append(`<option value="0-0-0">Default</option>`)
            initbootstrap();
        }


        mainfirst = false

    });

    // localize the parent child nodes with unique position identification
    function getparentbylevelandposition(node, level, index) {
        var parentname = node.name
        treedata.push({
            name: parentname,
            level: level + 1,
            index: index
        })
        node.children.forEach((child, index, arr) => {
            getparentbylevelandposition(child, level + 1, index)
        })
    }

    // clear the browser with erasing cache
    clear.click(function () {
        window.location.reload(true)
    })


    // click event of the push to DOM button
    domainpush.click(function () {

        //get the to be insert parent node level and index

        var selectedparentname = $('.parentlist').val().split("-")[0]
        var selectedparentlevel = parseInt($('.parentlist').val().split("-")[1].toString())
        var selectedparentindex = parseInt($('.parentlist').val().split("-")[2].toString())

        //id for the tag
        var selectedmainid = maintags.val();

       

        if (parentnode == null) {

            var atts = JSON.parse(JSON.stringify(tags.filter(t => t.id == selectedmainid)[0].attributes))

            // create parentnode
            var parent = new Object();
            parent.id = selectedmainid
            parent.name = tags.filter(t => t.id == selectedmainid)[0].name
            parent.attributes = atts

            parent.attributes.forEach(at => {
                at.defaultValue = $('#' + at.id).val()
            })

            parent.children = []
            console.log(parent)
            parentnode = parent;
        } else {
            // get the eligible parent for the child
            var parent = getparent(parentnode, selectedparentname, -1, 0, selectedparentlevel, selectedparentindex)

            var atts = JSON.parse(JSON.stringify(tags.filter(t => t.id == selectedmainid)[0].attributes))

            var child = new Object();
            child.name = tags.filter(t => t.id == selectedmainid)[0].name
            child.id = selectedmainid
            child.attributes = atts

            child.attributes.forEach(at => {
                at.defaultValue = $('#' + at.id).val()
            })

            // you can add the insert a text value inside the tag if wanted
            //child.text = "Fselect * from "

            child.children = []
            parent.children.push(child)

        }

        treedata = []

        // populate treedata array
        getparentbylevelandposition(parentnode, -1, 0)

        console.table(treedata)
        // init parent dropdown if it does not exist
        if (mainfirst) {
            injectparent.append(`<div class="form-group">
                                <label class="control-label">Parent Node</label>
                                <select class="form-control select2 parentlist"></select>
                              </div>`)
            var parentlist = $('.parentlist')
            // id-level-position
            parentlist.append(`<option></option>`)
            initbootstrap();
        }

        // clear the parent droppdown div
        $('.parentlist').empty()
        // populate parent node list in parent dropdown
        treedata.forEach(item => {
            $('.parentlist').append(`<option></option>`)
            $('.parentlist').append(`<option value="` + item.name + "-" + item.level + "-" + item.index + `">` + item.name + " level: " + item.level + " position: " + item.index + `</option>`)
        })

        // xml preview div clearing
        dom.empty()

        // beatify xml formatter import
        var format = require('xml-formatter');

        // convert the json to xml
        var xml = OBJtoXML(parentnode);

        // add indentation and format beautify
        dom.html("" + format(xml.replace(/<\/?[0-9]{1,}>/g, ''), {
            indentation: '                '
        }))

    })

})

// get the parent object of the child object
function getparent(node, name, level, index, originallevel, originalindex) {

    if (node.name == name && originallevel == (level + 1) && index == originalindex) {
        return node;
    }
    var selectedparent = null
    node.children.forEach((child, i, arr) => {
        var parent = getparent(child, child.name, level + 1, i, originallevel, originalindex)
        if (parent != null) {
            selectedparent = parent
        }
    })
    return selectedparent

}


// init jquery select2 
function initbootstrap() {
    $('.select2').select2({
        theme: 'bootstrap4',
        placeholder: "Select an option"
    });
}

// convert a json to xml
function OBJtoXML(obj) {
    var xml = ""
    xml += `<` + obj.name + " "

    obj.attributes.forEach(at => {
        xml += at.name + `="` + at.defaultValue + `" `
    })

    xml += `>`

    if (obj.text != null) {
        xml += obj.text
    }

    obj.children.forEach(child => {
        xml += OBJtoXML(child)
    })

    xml += `</` + obj.name + `>`

    return xml

}