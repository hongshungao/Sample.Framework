import { createRouter, createWebHistory } from 'vue-router'
import EntityList from '../views/EntityList.vue'
import EntityEdit from '../views/EntityEdit.vue'
import EnumList from '../views/EnumList.vue'
import EnumEdit from '../views/EnumEdit.vue'
import GenerateCode from '../views/GenerateCode.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/entities'
    },
    {
      path: '/entities',
      name: 'entities',
      component: EntityList
    },
    {
      path: '/entities/create',
      name: 'entity-create',
      component: EntityEdit
    },
    {
      path: '/entities/:id',
      name: 'entity-edit',
      component: EntityEdit,
      props: true
    },
    {
      path: '/enums',
      name: 'enums',
      component: EnumList
    },
    {
      path: '/enums/create',
      name: 'enum-create',
      component: EnumEdit
    },
    {
      path: '/enums/:id',
      name: 'enum-edit',
      component: EnumEdit,
      props: true
    },
    {
      path: '/generate',
      name: 'generate',
      component: GenerateCode
    }
  ]
})

export default router