<template>
  <div>
    <a-card title="代码生成" :bordered="false">
      <a-alert
        type="info"
        show-icon
        message="代码生成说明"
        description="您可以选择生成单个实体的代码，或者生成所有实体和枚举的代码。生成的代码将保存到项目的输出目录中。"
        style="margin-bottom: 16px"
      />

      <a-form layout="vertical">
        <a-form-item label="选择生成模式">
          <a-radio-group v-model:value="generateMode">
            <a-radio value="all">生成所有实体和枚举</a-radio>
            <a-radio value="single">生成单个实体</a-radio>
          </a-radio-group>
        </a-form-item>

        <a-form-item v-if="generateMode === 'single'" label="选择实体">
          <a-select
            v-model:value="selectedEntityId"
            placeholder="请选择要生成代码的实体"
            style="width: 100%"
            :loading="loading"
          >
            <a-select-option v-for="entity in entities" :key="entity.id" :value="entity.id">
              {{ entity.name }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item>
          <a-button type="primary" @click="generateCode" :loading="generating" :disabled="!canGenerate">
            <code-outlined /> 生成代码
          </a-button>
        </a-form-item>
      </a-form>

      <a-divider v-if="generationResult.length > 0">生成结果</a-divider>

      <a-result
        v-if="generationResult.length > 0"
        status="success"
        title="代码生成成功"
        sub-title="以下文件已成功生成"
      >
        <template #extra>
          <a-list
            bordered
            :data-source="generationResult"
            :render-item="(item) => ({ children: item })"
          />
        </template>
      </a-result>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { message } from 'ant-design-vue';
import { CodeOutlined } from '@ant-design/icons-vue';
import api from '../services/api';

const entities = ref([]);
const loading = ref(false);
const generating = ref(false);
const generateMode = ref('all');
const selectedEntityId = ref(null);
const generationResult = ref([]);

// 是否可以生成代码
const canGenerate = computed(() => {
  if (generateMode.value === 'all') return true;
  return !!selectedEntityId.value;
});

// 获取所有实体
const fetchEntities = async () => {
  loading.value = true;
  try {
    const response = await api.entity.getAll();
    entities.value = response.data;
    if (entities.value.length > 0 && !selectedEntityId.value) {
      selectedEntityId.value = entities.value[0].id;
    }
  } catch (error) {
    message.error('获取实体列表失败');
    console.error(error);
  } finally {
    loading.value = false;
  }
};

// 生成代码
const generateCode = async () => {
  if (!canGenerate.value) {
    message.warning('请先选择要生成代码的实体');
    return;
  }

  generating.value = true;
  try {
    let response;
    if (generateMode.value === 'single') {
      response = await api.entity.generateCode(selectedEntityId.value);
    } else {
      response = await api.entity.generateCode();
    }
    
    generationResult.value = response.data;
    message.success('代码生成成功');
  } catch (error) {
    message.error('代码生成失败: ' + (error.response?.data || error.message || '未知错误'));
    console.error(error);
  } finally {
    generating.value = false;
  }
};

onMounted(() => {
  fetchEntities();
});
</script>

<style scoped>
.ant-list {
  max-height: 300px;
  overflow-y: auto;
}
</style>