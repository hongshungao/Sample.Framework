<template>
  <a-layout class="layout">
    <a-layout-header>
      <div class="logo">代码生成框架</div>
      <a-menu
        v-model:selectedKeys="selectedKeys"
        theme="dark"
        mode="horizontal"
        :style="{ lineHeight: '64px' }"
      >
        <a-menu-item key="entities">
          <router-link to="/entities">实体管理</router-link>
        </a-menu-item>
        <a-menu-item key="enums">
          <router-link to="/enums">枚举管理</router-link>
        </a-menu-item>
        <a-menu-item key="generate">
          <router-link to="/generate">代码生成</router-link>
        </a-menu-item>
      </a-menu>
    </a-layout-header>
    <a-layout-content style="padding: 0 50px">
      <a-breadcrumb style="margin: 16px 0">
        <a-breadcrumb-item>首页</a-breadcrumb-item>
        <a-breadcrumb-item>{{ currentRoute }}</a-breadcrumb-item>
      </a-breadcrumb>
      <div class="site-layout-content">
        <router-view></router-view>
      </div>
    </a-layout-content>
    <a-layout-footer style="text-align: center">
      Sample Framework ©{{ new Date().getFullYear() }} Created with .NET Core 8.0 & Vue 3
    </a-layout-footer>
  </a-layout>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const selectedKeys = ref(['entities']);

const currentRoute = computed(() => {
  const path = route.path;
  if (path.includes('entities')) return '实体管理';
  if (path.includes('enums')) return '枚举管理';
  if (path.includes('generate')) return '代码生成';
  return '首页';
});
</script>

<style>
.layout {
  min-height: 100vh;
}

.logo {
  float: left;
  width: 120px;
  height: 31px;
  margin: 16px 24px 16px 0;
  color: white;
  font-size: 18px;
  font-weight: bold;
}

.site-layout-content {
  background: #fff;
  padding: 24px;
  min-height: 280px;
}
</style>